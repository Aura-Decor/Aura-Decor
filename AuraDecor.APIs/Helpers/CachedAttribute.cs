using System.Text;
using AuraDecor.Core.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuraDecor.APIs.Helpers;

public class CachedAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _timeToLifeInSeconed;
    private readonly string[]? _tags;
    
    public CachedAttribute(int timeToLifeInSeconed, params string[] tags)
    {
        _timeToLifeInSeconed = timeToLifeInSeconed;
        _tags = tags;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

        var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

        var cachedResponse = await cacheService.GetCacheResponseAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            var contentResult = new ContentResult()
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
            context.Result = contentResult;
            return;
        }

        var executedEndPointContext = await next.Invoke(); 
        if (executedEndPointContext.Result is OkObjectResult okObjectResult)
        {
            await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value,
                TimeSpan.FromSeconds(_timeToLifeInSeconed));
        }
    }
    
    private string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"{request.Path}");
        
        // Add tags to cache key for easier invalidation
        if (_tags != null && _tags.Length > 0)
        {
            keyBuilder.Append($"|tags:{string.Join(",", _tags)}");
        }
        
        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}