using Microsoft.AspNetCore.Mvc.Filters;

namespace AuraDecor.APIs.Helpers;

public enum RateLimitAlgorithm
{
    FixedWindow,
    SlidingWindow,
    TokenBucket
}

[AttributeUsage(AttributeTargets.Method)]
public class RateLimitAttribute : Attribute
{
    public int MaxRequests { get; }
    public int TimeWindowInSeconds { get; }
    public RateLimitAlgorithm Algorithm { get; set; }
    
    public RateLimitAttribute(int maxRequests, int timeWindowInSeconds, RateLimitAlgorithm algorithm = RateLimitAlgorithm.FixedWindow)
    {
        MaxRequests = maxRequests;
        TimeWindowInSeconds = timeWindowInSeconds;
        Algorithm = algorithm;
    }
}
