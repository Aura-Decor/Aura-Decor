using System.Net;
using System.Text.Json;
using AuraDecor.APIs.Errors;
using AuraDecor.APIs.Helpers;
using Microsoft.AspNetCore.Mvc.Controllers;
using StackExchange.Redis;

namespace AuraDecor.APIs.Middlewares;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RateLimitingMiddleware> _logger;

    public RateLimitingMiddleware(
        RequestDelegate next,
        IConnectionMultiplexer redis,
        ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _redis = redis;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor == null)
        {
            await _next(context);
            return;
        }

        var rateLimit = controllerActionDescriptor.MethodInfo
            .GetCustomAttributes(typeof(RateLimitAttribute), true)
            .FirstOrDefault() as RateLimitAttribute;

        if (rateLimit == null)
        {
            await _next(context);
            return;
        }

        var clientId = GetClientIdentifier(context);
        var key = $"rate-limit:{clientId}:{context.Request.Path}";
        var db = _redis.GetDatabase();

        var currentCount = await db.StringGetAsync(key);
        var requestCount = currentCount.IsNull ? 0 : int.Parse(currentCount.ToString());

        if (requestCount >= rateLimit.MaxRequests)
        {
            _logger.LogWarning($"Rate limit exceeded for client {clientId} on {context.Request.Path}");
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse(429, "you're banned for being gay");
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
            return;
        }

        if (requestCount == 0)
        {
            // Set initial value with expiry
            await db.StringSetAsync(key, 1, TimeSpan.FromSeconds(rateLimit.TimeInSeconds));
        }
        else
        {
            // Increment existing value
            await db.StringIncrementAsync(key);
        }

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                return userId;
            }
        }
        
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
