using System.Text.Json;
using AuraDecor.Core.Services.Contract;
using StackExchange.Redis;

namespace AuraDecor.Servicies;

public class ResponseCacheService : IResponseCacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _redis;
        
    public ResponseCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = redis.GetDatabase();
    }
    
    public async Task CacheResponseAsync(string key, object response, TimeSpan timeToLife)
    {
        if (response == null) return;

        var options = new JsonSerializerOptions() { PropertyNamingPolicy= JsonNamingPolicy.CamelCase };
        var jsonSerializer = JsonSerializer.Serialize(response, options);

        await _database.StringSetAsync(key, jsonSerializer, timeToLife);
    }

    public async Task<string> GetCacheResponseAsync(string cacheKey)
    {
        var cachedResponse = await _database.StringGetAsync(cacheKey);
        if (cachedResponse.IsNullOrEmpty) return null;
        return cachedResponse;
    }

    public async Task InvalidateCacheAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task InvalidateCachePatternAsync(string pattern)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);
            
            if (keys.Any())
            {
                await _database.KeyDeleteAsync(keys.ToArray());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cache invalidation failed for pattern {pattern}: {ex.Message}");
        }
    }

    public async Task InvalidateTaggedCacheAsync(string tag)
    {
        await InvalidateCachePatternAsync($"*{tag}*");
    }

    public async Task ClearAllCacheAsync()
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            await server.FlushDatabaseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Clear all cache failed: {ex.Message}");
        }
    }

    public async Task InvalidateMultiplePatternsAsync(params string[] patterns)
    {
        var tasks = patterns.Select(pattern => InvalidateCachePatternAsync(pattern));
        await Task.WhenAll(tasks);
    }

    public async Task InvalidateCacheByKeyListAsync(IEnumerable<string> keys)
    {
        if (keys?.Any() == true)
        {
            await _database.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());
        }
    }

    public async Task<bool> CacheExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task SetCacheExpirationAsync(string key, TimeSpan expiration)
    {
        await _database.KeyExpireAsync(key, expiration);
    }
}