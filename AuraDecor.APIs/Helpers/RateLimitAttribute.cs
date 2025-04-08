using Microsoft.AspNetCore.Mvc.Filters;

namespace AuraDecor.APIs.Helpers;

[AttributeUsage(AttributeTargets.Method)]
public class RateLimitAttribute : Attribute
{
    public int MaxRequests { get; }
    public int TimeInSeconds { get; }
    
    public RateLimitAttribute(int maxRequests, int timeInSeconds)
    {
        MaxRequests = maxRequests;
        TimeInSeconds = timeInSeconds;
    }
}
