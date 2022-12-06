using System.Diagnostics;

namespace Financial.Middleware
{
    public class BrowserMiddleware
    {
        private readonly RequestDelegate _next;

        public BrowserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, ILogger<DateLogMiddleware> logger)
        {
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
            var ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
            var url = httpContext.Request.Path;
            logger.LogInformation("userAgent: " + userAgent);
            logger.LogInformation("ipAddress: " + ipAddress);
            logger.LogInformation("url: " + url);
            return _next(httpContext);
        }
    }

    public static class BrowserMiddlewareExtensions
    {
        public static IApplicationBuilder UseBrowserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BrowserMiddleware>();
        }
    }
}

