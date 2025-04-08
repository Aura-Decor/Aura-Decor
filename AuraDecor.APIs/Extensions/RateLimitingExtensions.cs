using AuraDecor.APIs.Middlewares;

namespace AuraDecor.APIs.Extensions;

public static class RateLimitingExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitingMiddleware>();
    }
}
