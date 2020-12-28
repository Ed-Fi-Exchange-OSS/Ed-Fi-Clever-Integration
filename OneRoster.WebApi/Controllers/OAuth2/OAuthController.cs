using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using EdFi.OneRoster.WebApi.Services;

namespace EdFi.OneRoster.WebApi.Controllers.OAuth2
{
    [Route("[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly ILogger<OAuthController> _logger;
        private readonly IConfiguration _config;
        private readonly IIdentityService _service;

        public OAuthController(ILogger<OAuthController> logger, IConfiguration config
            , IIdentityService service
            )
        {
            _logger = logger;
            _config = config;
            _service = service;
        }

        [HttpPost("token"), AllowAnonymous]
        public async Task<ActionResult<string>> Token([FromForm] OAuthRequest request)
        {
            if (request.Grant_type != "client_credentials")
                return Unauthorized($"grant_type ({request.Grant_type}) not supported");
            var client_id = "";
            var client_secret = "";
            // We implemented the standard https://tools.ietf.org/html/rfc6749#page-40
            // Where Basic auth includes the client_id and client_secret base64 encoded
            // "Basic btoa(client_id:client_secret)"

            var authHeader = Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(authHeader))
            {
                client_id = request.Client_id;
                client_secret = request.Client_secret;
            }
            else
            {// Get strongly typed auth header
                var auth = AuthenticationHeaderValue.Parse(authHeader);

                if (auth == null)
                    return Unauthorized("No 'Authorization' header present.");

                if (!auth.Scheme.Equals("Basic", StringComparison.InvariantCultureIgnoreCase))
                    return Unauthorized("Non supported scheme.");

                // We need the Auth paramenter as it contains the client_id and client_secret
                if (string.IsNullOrEmpty(auth.Parameter))
                    return Unauthorized("No value in the Authorization header.");

                var apiClientInfo = Base64Decode(auth.Parameter).Split(":");
                client_id = apiClientInfo[0];
                client_secret = apiClientInfo[1];
            }

            if (string.IsNullOrEmpty(client_id))
            {
                return Unauthorized("Client Id not present.");
            }

            // If we have the token in our DB then this is a valid API Client
            var identity = await _service.GetIdentity(client_id, client_secret);
            //var identity = await _service.GetIdentity(request.Client_id, request.Client_secret);

            if (identity == null)
                return Unauthorized($"Key and Secret combination are invalid");

            var claims = new List<Claim> {
                //new Claim("APIClientId", identity.Id.ToString()),
                //new Claim("NelticClientId", identity.NelticClientId.ToString()),
                //// TODO: Implement logic to make this so.
                //// These are just ideas but we dont need this right now...
                //new Claim("ViewAllOrders", "True"),
                //// Add actions like (Get,Post,Put,Delete)
                //new Claim("AccessToEndpoints", "['*Types','/PurchaseOrders','/Projects','/...']"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var accessTokenExpireTime = Convert.ToInt32(_config["Jwt:AccessToken:ExpiresInMinutes"]);
            var refreshTokenExpireTime = Convert.ToInt32(_config["Jwt:RefreshToken:ExpiresInDays"]);

            var token = new JwtSecurityToken(
                  _config["Jwt:Issuer"],
                  _config["Jwt:Audience"],
                  expires: DateTime.Now.AddMinutes(accessTokenExpireTime),
                  claims: claims,
                  signingCredentials: creds
              );

            var refreshToken = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                expires: DateTime.Now.AddDays(refreshTokenExpireTime),
                claims: claims,
                signingCredentials: creds
            );

            var model = new OAuthResponse
            {
                Access_token = await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(token)),
                Expires_in = accessTokenExpireTime * 60, // Returning seconds - Based on the specification https://tools.ietf.org/html/rfc6749#page-30
                Token_type = "Bearer",
                Refresh_token = await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(refreshToken)),
            };

            return Ok(model);
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
