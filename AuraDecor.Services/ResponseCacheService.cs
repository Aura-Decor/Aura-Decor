using System.Text.Json;
using AuraDecor.Core.Services.Contract;
using StackExchange.Redis;

namespace AuraDecor.Servicies;

public class ResponseCacheService : IResponseCacheService
{
    private readonly IDatabase _database;
        
    public ResponseCacheService(IConnectionMultiplexer redis)
    {
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
}