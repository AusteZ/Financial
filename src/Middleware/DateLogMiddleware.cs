using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Financial.Middleware
{
    public class DateLogMiddleware
    {
        private readonly RequestDelegate _next;

        public DateLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, ILogger<DateLogMiddleware> logger)
        {
            logger.LogInformation("Date: " + DateTime.Now.ToLongDateString());
            return _next(httpContext);
        }
    }

    public static class DateLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseDateLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DateLogMiddleware>();
        }
    }
}