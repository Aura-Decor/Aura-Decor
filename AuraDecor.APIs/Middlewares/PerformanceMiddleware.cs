using System.Diagnostics;

namespace AuraDecor.APIs.Middlewares;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path.Value;
        var method = context.Request.Method;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            var statusCode = context.Response.StatusCode;

            // Log slow requests (over 1 second)
            if (elapsedMs > 1000)
            {
                _logger.LogWarning(
                    "Slow request: {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    method, requestPath, statusCode, elapsedMs);
            }
            else if (elapsedMs > 500)
            {
                _logger.LogInformation(
                    "Request: {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    method, requestPath, statusCode, elapsedMs);
            }

            // Add performance headers for monitoring
            context.Response.Headers.Append("X-Response-Time", $"{elapsedMs}ms");
        }
    }
}