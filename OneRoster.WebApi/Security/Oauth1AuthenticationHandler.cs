using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace EdFi.OneRoster.WebApi.Security
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    public class Oauth1AuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        //private readonly ICustomAuthenticationManager customAuthenticationManager;
        private readonly ILogger _logger;

        private readonly ApplicationSettings _settings;

        public Oauth1AuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory loggerFactory, UrlEncoder encoder, ISystemClock clock, IOptions<ApplicationSettings> settings  ) : base(options, loggerFactory, encoder, clock)
        {

            //this.customAuthenticationManager = customAuthenticationManager;
            _logger = loggerFactory.CreateLogger<Oauth1AuthenticationHandler>();
            _settings = settings.Value;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                return Validate();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex.Message);
                this.Context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Context.Response.WriteAsync(ex.Message);
                return AuthenticateResult.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                Context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await Context.Response.WriteAsync(ex.Message);
                return AuthenticateResult.Fail(ex.Message);
            }
           

        }
        private AuthenticateResult Validate()
        {

            if (_settings.OAuthEnabled)
                ValidateAuth(this.Context);
            
            var principal = new System.Security.Principal.GenericPrincipal(new ClaimsIdentity(), null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        public void ValidateAuth(HttpContext context)
        {
            var auth = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(auth))
                throw new UnauthorizedAccessException("Authorization not  found");

            var parameters = GetOauthParameters(auth);
            if (!parameters.ContainsKey("oauth_consumer_key"))
                throw new UnauthorizedAccessException("Unauthorized, consumer key (clientId) is missing.");

            var consumerKey = parameters.FirstOrDefault(m => m.Key == "oauth_consumer_key");
            var clients = _settings.Clients;
            if (!clients.ContainsKey(consumerKey.Value))
                throw new UnauthorizedAccessException("Unauthorized, consumer key (clientId) is missing.");

            var queryParam = context.Request.QueryString.ToString().Replace("?", "").Split("&").Where(m => m != "").ToList();
            var parametersBuild = GetOauthParametersBuild(auth);

            parametersBuild.AddRange(queryParam);

            var client = clients.FirstOrDefault(m => m.Key == consumerKey.Value);
            var clientSecret = client.Value;

            var parametersString = ConvertParametersToString(parametersBuild);

            var authUrl = GetRequestedUrl(context);
            var signatureBaseString = "GET&" + Uri.EscapeDataString(authUrl) + "&" + Uri.EscapeDataString(parametersString);

            var requestSignature = parameters.First(m => m.Key == "oauth_signature").Value;

            _logger.LogDebug("Client signature " + requestSignature);

            var result = ValidateOauthSignature(requestSignature, clientSecret, signatureBaseString);
        }

        private string GetRequestedUrl(HttpContext context)
        {
            var indexOf = context.Request.GetDisplayUrl().IndexOf("?", StringComparison.Ordinal);

            indexOf = indexOf < 0 ? context.Request.GetDisplayUrl().Length : indexOf;

            return context.Request.GetDisplayUrl().Substring(0, indexOf);
        }

        private List<string> GetOauthParametersBuild(string authHeader)
        {
            var parameters = new List<string>();

            authHeader.Split(',').ToList().ForEach(headerPart =>
            {
                var authParam = GetOauthParameterBuild(headerPart);

                if (authParam != "")
                {
                    parameters.Add(authParam);
                }
            });

            return parameters;
        }

        private string GetOauthParameterBuild(string headerPart)
        {
            if (!headerPart.Contains('='))
            {
                return "";
            }
            var keyValue = headerPart.Split("=");

            var key = keyValue[0].Trim();

            if (key.Contains("OAuth "))
            {
                key = key.Replace("OAuth ", "");
            }

            var value = keyValue[1].Replace("\\", "").Replace("\"", "").Replace("/\\/g", " ");

            return key + "=" + value;
        }

        private string ConvertParametersToString(List<string> parametersBuild)
        {
            parametersBuild = parametersBuild.OrderBy(m => m).ToList();

            var parametersString = "";

            var amount = 0;
            foreach (var item in parametersBuild)
            {

                if (item.Contains("oauth_signature") && !item.Contains("oauth_signature_method"))
                {
                    amount++;
                    continue;
                }
                parametersString += item;

                if (amount < parametersBuild.Count - 1)
                    parametersString += '&';

                amount++;

            }

            return parametersString;
        }

        private Dictionary<string, string> GetOauthParameters(string authHeader)
        {
            var authParameters = new Dictionary<string, string>();

            authHeader.Split(',').ToList().ForEach(headerPart =>
            {
                var authParam = GetOauthParameter(headerPart);

                if (authParam.Key != null && authParam.Key != "")
                {
                    authParameters.Add(authParam.Key, authParam.Value);
                }
            });

            return authParameters;
        }

        private KeyValuePair<string, string> GetOauthParameter(string headerPart)
        {

            if (!headerPart.Contains('='))
            {
                return new KeyValuePair<string, string>();
            }

            var keyValue = headerPart.Split("=");

            var key = keyValue[0].Trim();
            if (key.Contains("OAuth "))
            {
                key = key.Replace("OAuth ", "");
            }

            var value = keyValue[1].Replace("\\", "").Replace("\"", "").Replace("/\\/g", " ");

            return new KeyValuePair<string, string>(key, value);
        }

        private bool ValidateOauthSignature(string requestSignature, string clientSecret, string signatureBaseString)
        {
            var generatedSignature = CreateOauthSignature(clientSecret, signatureBaseString);

            _logger.LogDebug("generated signature " + generatedSignature);

            if (requestSignature != generatedSignature)
                throw new UnauthorizedAccessException("Unauthorized, invalid auth signature.");
            return true;
        }

        private string CreateOauthSignature(string clientSecret, string signatureBaseString)
        {
            // & needed by oauth1 convention
            var keybytes = Encoding.Default.GetBytes(clientSecret + "&");

            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keybytes);

            var value = cryptographer.ComputeHash(Encoding.Default.GetBytes(signatureBaseString));

            string digest = System.Convert.ToBase64String(value).Replace("-", "");

            string formated = Uri.EscapeDataString(digest);

            return formated;
        }
    }


}
