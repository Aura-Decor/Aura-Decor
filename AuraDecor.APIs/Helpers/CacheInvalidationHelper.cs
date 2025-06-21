using AuraDecor.Core.Services.Contract;

namespace AuraDecor.APIs.Helpers;

public static class CacheInvalidationHelper
{
    // Cache key patterns for different resources
    public const string FURNITURE_CACHE_PATTERN = "/api/furniture*";
    public const string FURNITURE_OFFERS_CACHE_PATTERN = "/api/furniture/offers*";
    public const string FURNITURE_BY_ID_CACHE_PATTERN = "/api/furniture/{0}";
    public const string CART_CACHE_PATTERN = "/api/cart*";
    public const string USER_CACHE_PATTERN = "/api/account*";
    public const string ORDER_CACHE_PATTERN = "/api/order*";
    public const string PAYMENT_CACHE_PATTERN = "/api/payment*";
    public const string ADMIN_CACHE_PATTERN = "/api/admin*";

    public static async Task InvalidateFurnitureCacheAsync(IResponseCacheService cacheService)
    {
        // Invalidate all furniture-related cache
        await cacheService.InvalidateCachePatternAsync(FURNITURE_CACHE_PATTERN);
    }

    public static async Task InvalidateFurnitureByIdCacheAsync(IResponseCacheService cacheService, Guid furnitureId)
    {
        // Invalidate specific furniture item cache
        var cacheKey = string.Format(FURNITURE_BY_ID_CACHE_PATTERN, furnitureId);
        await cacheService.InvalidateCacheAsync(cacheKey);
    }

    public static async Task InvalidateOffersCacheAsync(IResponseCacheService cacheService)
    {
        // Invalidate offers-related cache
        await cacheService.InvalidateCachePatternAsync(FURNITURE_OFFERS_CACHE_PATTERN);
        // Also invalidate general furniture cache since offers affect listings
        await cacheService.InvalidateCachePatternAsync(FURNITURE_CACHE_PATTERN);
    }

    public static async Task InvalidateCartCacheAsync(IResponseCacheService cacheService)
    {
        // Invalidate cart-related cache
        await cacheService.InvalidateCachePatternAsync(CART_CACHE_PATTERN);
    }

    public static async Task InvalidateUserCacheAsync(IResponseCacheService cacheService)
    {
        // Invalidate user-related cache
        await cacheService.InvalidateCachePatternAsync(USER_CACHE_PATTERN);
    }

    public static async Task InvalidateOrderCacheAsync(IResponseCacheService cacheService)
    {
        // Invalidate order-related cache
        await cacheService.InvalidateCachePatternAsync(ORDER_CACHE_PATTERN);
    }

    public static async Task InvalidatePaymentCacheAsync(IResponseCacheService cacheService)
    {
        // Invalidate payment-related cache
        await cacheService.InvalidateCachePatternAsync(PAYMENT_CACHE_PATTERN);
    }

    public static async Task InvalidateAdminCacheAsync(IResponseCacheService cacheService)
    {
        // Invalidate admin-related cache
        await cacheService.InvalidateCachePatternAsync(ADMIN_CACHE_PATTERN);
    }

    public static async Task InvalidateUserSpecificCacheAsync(IResponseCacheService cacheService, string userId)
    {
        // Invalidate user-specific cache patterns
        await cacheService.InvalidateCachePatternAsync($"*{userId}*");
        await cacheService.InvalidateCachePatternAsync(CART_CACHE_PATTERN);
        await cacheService.InvalidateCachePatternAsync(ORDER_CACHE_PATTERN);
    }

    public static async Task InvalidateInventoryRelatedCacheAsync(IResponseCacheService cacheService)
    {
        // Invalidate cache that affects inventory
        await cacheService.InvalidateCachePatternAsync(FURNITURE_CACHE_PATTERN);
        await cacheService.InvalidateCachePatternAsync(FURNITURE_OFFERS_CACHE_PATTERN);
    }
}
