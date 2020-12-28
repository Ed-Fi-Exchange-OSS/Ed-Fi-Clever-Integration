using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EdFi.OneRoster.WebApi.Security
{    

    public class OAuth1Options : AuthenticationSchemeOptions
    {
        public List<OAuth1Client> oAuth1Clients { get; set; }

        public const string DefaultScheme = OAuth1Defaults.AuthenticationScheme;
        public string Scheme => DefaultScheme;
    }

    public class OAuth1AuthenticationHandler : AuthenticationHandler<OAuth1Options>
    {
        private readonly ILogger _logger;
        private readonly ApplicationSettings _settings;


        public OAuth1AuthenticationHandler(
            IOptionsMonitor<OAuth1Options> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, 
            IOptions<ApplicationSettings> settings

            )
            : base(options, logger, encoder, clock)
        {
            this._settings = settings.Value;
            this._logger = logger.CreateLogger("OAuth1AuthenticationHandler");
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {

                return Validate();
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private AuthenticateResult Validate()
        {
            if (_settings.OAuthEnabled)
                return ValidateAuth(this.Context);

            var identity = new ClaimsIdentity(new List<Claim>(), Scheme.Name);
            var principal = new GenericPrincipal(identity, new[] { "" });
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        private AuthenticateResult ReturnValidResponse()
        {

            var claims = new List<Claim>
            {
                //new Claim(ClaimTypes.Name, ""),
                //new Claim(ClaimTypes.Role, "")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new GenericPrincipal(identity, new[] { "" });
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }


        public AuthenticateResult ValidateAuth(HttpContext context)
        {
            var auth = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(auth))
                throw new UnauthorizedAccessException("Authorization not found");

            var parameters = GetOauthParameters(auth);
            if (!parameters.ContainsKey("oauth_consumer_key"))
                throw new UnauthorizedAccessException("Unauthorized, consumer key (clientId) is missing.");

            var consumerKey = parameters.FirstOrDefault(m => m.Key == "oauth_consumer_key");

            var clients = Options.oAuth1Clients;

            if (!clients.Any(m => m.Client_Id == consumerKey.Value))
                throw new UnauthorizedAccessException("Unauthorized, consumer key (clientId) is missing.");

            var queryParam = context.Request.QueryString.ToString().Replace("?", "").Split("&").Where(m => m != "").ToList();
            var parametersBuild = GetOauthParametersBuild(auth);

            parametersBuild.AddRange(queryParam);

            var client = clients.FirstOrDefault(m => m.Client_Id == consumerKey.Value);
            var clientSecret = client.Client_Secret;

            var parametersString = ConvertParametersToString(parametersBuild);

            var authUrl = GetRequestedUrl(context);
            var signatureBaseString = "GET&" + Uri.EscapeDataString(authUrl) + "&" + Uri.EscapeDataString(parametersString);

            var requestSignature = parameters.First(m => m.Key == "oauth_signature").Value;

            _logger.LogDebug("Client signature " + requestSignature);

            var result = ValidateOauthSignature(requestSignature, clientSecret, signatureBaseString);

            return ReturnValidResponse();
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

    public static class OAuth1Extensions
    {
        public static AuthenticationBuilder AddOAuth1(this AuthenticationBuilder builder, Action<OAuth1Options> configureOptions)
        {
            // Add custom authentication scheme with custom options and custom handler
            return builder.AddScheme<OAuth1Options, OAuth1AuthenticationHandler>(OAuth1Options.DefaultScheme, configureOptions);
        }

        public static AuthenticationBuilder AddOAuth1(this AuthenticationBuilder builder)
            => builder.AddOAuth1("OAuth1", _ => { });

        public static AuthenticationBuilder AddOAuth1(this AuthenticationBuilder builder, string authenticationScheme, Action<OAuth1Options> configureOptions)
            => builder.AddOAuth1(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddOAuth1(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<OAuth1Options> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OAuth1Options>, OAuth1PostConfigureOptions>());
            return builder.AddScheme<OAuth1Options, OAuth1AuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }

    public class OAuth1PostConfigureOptions : IPostConfigureOptions<OAuth1Options>
    {
        /// <summary>
        /// Invoked to post configure a JwtBearerOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, OAuth1Options options)
        {

        }
    }

    public class OAuth1Client
    {
        public string Client_Id { get; set; }
        public string Name { get; set; }
        public string Client_Secret { get; set; }

    }

    /// <summary>
    /// Default values used by bearer authentication.
    /// </summary>
    public static class OAuth1Defaults
    {
        /// <summary>
        /// Default value for AuthenticationScheme property in the JwtBearerAuthenticationOptions
        /// </summary>
        public const string AuthenticationScheme = "OAuth1";
    }

}
