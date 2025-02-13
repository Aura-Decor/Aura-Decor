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
            _logger.LogError(ex, ex.Message); //Development
            // log the exception in (Database, File, Cloud) //Production
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            var response = _env.IsDevelopment()?
                new ApiExceptionResponse((int) HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                : new ApiExceptionResponse((int) HttpStatusCode.InternalServerError);
            
            var options = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            var json = JsonSerializer.Serialize(response, options);
            await httpContext.Response.WriteAsync(json);
        }
        

    }
    
}