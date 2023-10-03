using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
namespace BiometricDataFetchAPI
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log controller and action information
            //var controller = context.Request.HttpContext.GetRouteValue("controller");
            // Continue processing the request
            await _next(context);
            var controller = context.GetRouteValue("controller")?.ToString();

            var action = context.GetRouteValue("action")?.ToString();

            _logger.LogInformation($"#Controller: {controller},#Action: {action} visited : {DateTime.Now}");
        }
    }
}
