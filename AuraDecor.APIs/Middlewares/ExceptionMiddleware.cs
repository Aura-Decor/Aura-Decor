using System.Net;
using System.Text.Json;
using AuraDecor.APIs.Errors;

namespace AuraDecor.APIs.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionMiddleware> _logger;

    private readonly IHostEnvironment _env;
    // By Convention

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next.Invoke(httpContext);
        }
        catch (Exception ex)
        {
            // Log the exception with structured logging
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestId"] = httpContext.TraceIdentifier,
                ["RequestPath"] = httpContext.Request.Path.ToString(),
                ["RequestMethod"] = httpContext.Request.Method,
                ["UserAgent"] = httpContext.Request.Headers.UserAgent.ToString()
            }))
            {
                _logger.LogError(ex, "Unhandled exception occurred");
            }

            // Set response properties
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create appropriate response based on environment
            var response = _env.IsDevelopment()
                ? new ApiExceptionResponse(
                    (int)HttpStatusCode.InternalServerError, 
                    ex.Message, 
                    ex.StackTrace?.ToString())
                : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

            var options = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _env.IsDevelopment()
            };
            
            var json = JsonSerializer.Serialize(response, options);
            await httpContext.Response.WriteAsync(json);
        }
    }
    
}