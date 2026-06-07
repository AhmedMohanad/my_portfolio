using Portofolio.LoggingServices;

namespace Portofolio.MiddleWares
{
    public class RequestLoggerMiddleware : IRequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISimpleLogger _logger;

        public RequestLoggerMiddleware(RequestDelegate next, ISimpleLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log request
            _logger.LogInfo($"REQUEST: {context.Request.Method} {context.Request.Path}");

            var startTime = DateTime.Now;

            try
            {
                await _next(context);

                var duration = (DateTime.Now - startTime).TotalMilliseconds;

                // Log response
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                    _logger.LogSuccess($"RESPONSE: {context.Request.Method} {context.Request.Path} - Status: {context.Response.StatusCode} - Duration: {duration}ms");
                else if (context.Response.StatusCode >= 400)
                    _logger.LogError($"RESPONSE: {context.Request.Method} {context.Request.Path} - Status: {context.Response.StatusCode} - Duration: {duration}ms");
                else
                    _logger.LogInfo($"RESPONSE: {context.Request.Method} {context.Request.Path} - Status: {context.Response.StatusCode} - Duration: {duration}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"REQUEST FAILED: {context.Request.Method} {context.Request.Path}");
                throw;
            }
        }
    }
}

