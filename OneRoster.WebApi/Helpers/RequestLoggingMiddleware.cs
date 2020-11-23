using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EdFi.OneRoster.WebApi.Helpers
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ApplicationSettings _settings;
        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<ApplicationSettings> settings)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
            _settings = settings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation(
                 "Request {url} {QueryString} => {statusCode}",
                 context.Request?.Path.Value,
                 context.Request?.QueryString,
                 context.Response?.StatusCode);

            await _next(context);
        }
    }
}