namespace AuraDecor.APIs.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets a required connection string and throws if not found
    /// </summary>
    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        var connectionString = configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{name}' is required but not found in configuration.");
        }
        return connectionString;
    }

    /// <summary>
    /// Gets a required configuration value and throws if not found
    /// </summary>
    public static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Configuration value '{key}' is required but not found.");
        }
        return value;
    }

    /// <summary>
    /// Gets configuration value with a default fallback
    /// </summary>
    public static string GetValueOrDefault(this IConfiguration configuration, string key, string defaultValue)
    {
        return configuration[key] ?? defaultValue;
    }

    /// <summary>
    /// Gets configuration value as integer with default fallback
    /// </summary>
    public static int GetIntValueOrDefault(this IConfiguration configuration, string key, int defaultValue)
    {
        var value = configuration[key];
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Gets configuration value as boolean with default fallback
    /// </summary>
    public static bool GetBoolValueOrDefault(this IConfiguration configuration, string key, bool defaultValue)
    {
        var value = configuration[key];
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }
}