#region Using Directives
using AuraDecor.APIs.Extensions;
using AuraDecor.APIs.Middlewares;
using AuraDecor.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using HealthChecks.UI.Client;
using Stripe;


#endregion

#region Builder Configuration
var builder = WebApplication.CreateBuilder(args);

// Configure host options for better performance
builder.Host.ConfigureHostOptions(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

// Configure Kestrel for better performance
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB limit
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(120);
});

builder.Services.AddCors();

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSwaggerServices();
#endregion

#region Application Configuration
var app = builder.Build();

#region DatabaseMigration
// Optimize database migration with better error handling and performance
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        
        // Check if migration is needed before attempting
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying {Count} pending migrations", pendingMigrations.Count());
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully");
        }
        else
        {
            logger.LogInformation("Database is up to date, no migrations needed");
        }

        // Seed data only if necessary
        await AppDbContextDataSeed.SeedAsync(dbContext);
        logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database migration or seeding");
        
        // In production, you might want to fail fast on migration errors
        if (!app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}
#endregion

// Configure security and performance middleware in optimal order
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<PerformanceMiddleware>();

// Add security headers for production
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.Use(next => context =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        return next(context);
    });
}

app.UseRateLimiting();
app.UseSwaggerMiddleWare();
app.MapScalarApiReference(options =>
    options
        .WithTheme(ScalarTheme.BluePlanet)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
);

app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.UseHttpsRedirection();

// Add response caching middleware
app.UseResponseCaching();
app.UseOutputCache();

app.UseStaticFiles();

// Configure Stripe with environment-specific settings
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

if (app.Environment.IsDevelopment())
{
    StripeConfiguration.AppInfo = new AppInfo
    {
        Name = "AuraDecor Dev Environment",
        Version = "0.1.0",
    };
}

// Optimize CORS configuration
if (app.Environment.IsDevelopment())
{
    // Development: Allow all origins for easier testing
    app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
}
else
{
    // Production: Use specific origins for security
    app.UseCors(c => c
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost" })
        .AllowCredentials());
}



app.UseAuthentication();
app.UseAuthorization();

// Configure optimized health checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false
});

// Separate lightweight health check for load balancers
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    AllowCachingResponses = false
});

app.MapHealthChecksUI(options =>
{
    options.ResourcesPath = "/health-ui/resources";
    options.UIPath = "/health-ui";
});

app.MapControllers();

// Configure graceful shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application is shutting down gracefully...");
});

app.Run();
#endregion