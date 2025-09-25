using AuraDecor.APIs.Errors;
using AuraDecor.APIs.Helpers;
using AuraDecor.Core.Configuration;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Repository;
using AuraDecor.Repository.Data;
using AuraDecor.Services;
using AuraDecor.Servicies;
using HealthChecks.UI.Client;
using HealthChecks.UI.Core.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace AuraDecor.APIs.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IFurnitureService, FurnitureService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IResponseCacheService, ResponseCacheService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IStyleService, StyleService>();
        services.AddScoped<IColorService, ColorService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
        
        services.AddScoped<FurniturePicUrlResolver>();
        services.AddScoped<FurnitureModdelUrlResolver>();

        services.AddScoped<CartItemPicUrlResolver>();
        
        services.AddHttpClient();
        
        services.AddAutoMapper(m => m.AddProfile<MappingProfiles>());
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(config.GetRequiredConnectionString("DefaultConnection"), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                sqlOptions.CommandTimeout(30);
            });
            
            // Enable query caching for better performance
            options.EnableServiceProviderCaching();
            options.EnableSensitiveDataLogging(false);
            
            // Configure change tracking for better performance
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        // Configure Redis with optimized settings
        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetRequiredConnectionString("Redis");

            var configOptions = ConfigurationOptions.Parse(connectionString, true);
            configOptions.AbortOnConnectFail = false;
            configOptions.ConnectRetry = 3;
            configOptions.ConnectTimeout = 5000;
            configOptions.SyncTimeout = 1000;
            configOptions.KeepAlive = 60;
            
            return ConnectionMultiplexer.Connect(configOptions);
        });


        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage).ToArray();

                var errorResponse = new ApiValidationErrorResponse
                {
                    Errors = errors
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });
        
        services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
        services.Configure<RabbitMqSettings>(config.GetSection("RabbitMQ"));
        
        // Add response caching services
        services.AddResponseCaching(options =>
        {
            options.SizeLimit = 100 * 1024 * 1024; // 100MB cache limit
            options.MaximumBodySize = 64 * 1024; // Cache responses up to 64KB
        });

        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder => builder.Cache());
            options.AddPolicy("furniture-cache", builder => 
                builder.Cache()
                    .Expire(TimeSpan.FromMinutes(5))
                    .SetVaryByQuery("page", "pageSize", "categoryId", "brandId"));
        });

        services.AddHostedService<EmailQueueConsumer>();

        // Optimize health checks configuration
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(
                name: "Database",
                tags: new[] { "ready", "db" })
            .AddRedis(
                config.GetRequiredConnectionString("Redis"),
                name: "Redis",
                tags: new[] { "ready", "cache" })
            .AddRabbitMQ(
                config.GetRequiredValue("RabbitMQ:Uri"),
                name: "RabbitMQ",
                tags: new[] { "ready", "messaging" });

        services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(30); // Increased from 10 to reduce load
            options.MaximumHistoryEntriesPerEndpoint(100); // Increased from 60 for better history
            options.AddHealthCheckEndpoint("AuraDecor API", "/health");
        }).AddInMemoryStorage();

        return services;
    }
}