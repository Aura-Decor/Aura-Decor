namespace AuraDecor.Core.Services.Contract;

public interface IResponseCacheService
{
    Task CacheResponseAsync(string key, object response, TimeSpan timeToLife);
    Task<string> GetCacheResponseAsync(string cacheKey);
    
    // Basic cache invalidation methods
    Task InvalidateCacheAsync(string key);
    Task InvalidateCachePatternAsync(string pattern);
    Task InvalidateTaggedCacheAsync(string tag);
    Task ClearAllCacheAsync();

    // Advanced cache invalidation methods
    Task InvalidateMultiplePatternsAsync(params string[] patterns);
    Task InvalidateCacheByKeyListAsync(IEnumerable<string> keys);
    Task<bool> CacheExistsAsync(string key);
    Task SetCacheExpirationAsync(string key, TimeSpan expiration);
}