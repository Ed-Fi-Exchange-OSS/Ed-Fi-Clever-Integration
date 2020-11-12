using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EdFi.OneRoster.WebApi.Security
{
    public class OAuth1Middleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ApplicationSettings _settings;
        public OAuth1Middleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<ApplicationSettings> settings)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
            _settings = settings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (_settings.OAuthEnabled)
                    ValidateAuth(context);

                await _next(context);
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.LogError(e.Message);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(e.Message);
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(e.Message);
                return;
            }
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
            var requestedUri = new UriBuilder
            {
                Host = context.Request.Host.Host,
                Scheme = context.Request.Scheme,
                Path = context.Request.Path
            };
            if (context.Request.Host.Port != null)
                requestedUri.Port = context.Request.Host.Port.Value;

            return requestedUri.ToString();
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
