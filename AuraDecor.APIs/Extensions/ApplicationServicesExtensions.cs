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
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
        services.AddAutoMapper(m => m.AddProfile<MappingProfiles>());
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(c =>
        {
            var configuration = ConfigurationOptions.Parse(config.GetConnectionString("Redis"), true);
            return ConnectionMultiplexer.Connect(configuration);
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
        
        services.AddHostedService<EmailQueueConsumer>();

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(name: "Database")
            .AddRedis(config.GetConnectionString("Redis"), name: "Redis")
            .AddRabbitMQ(config["RabbitMQ:Uri"], name: "RabbitMQ");

        services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(10); 
            options.MaximumHistoryEntriesPerEndpoint(60); 
            options.AddHealthCheckEndpoint("AuraDecor API", "/health");
        }).AddInMemoryStorage();

        return services;
    }
}