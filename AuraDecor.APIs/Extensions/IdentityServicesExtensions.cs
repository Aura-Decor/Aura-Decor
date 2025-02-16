using System.Text;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Repository.Data;
using AuraDecor.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuraDecor.APIs.Extensions;

public static class IdentityServicesExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped(typeof(ITokenService), typeof(TokenService));
        services.AddIdentity<User,IdentityRole>(options =>
        {
            // options.Password.RequireNonAlphanumeric = false;
            // options.Password.RequireDigit = false;
            // options.Password.RequireLowercase = false;
            // options.Password.RequireUppercase = false;
            // options.Password.RequiredLength = 6;
        }).AddEntityFrameworkStores<AppDbContext>();
        services.AddAuthentication( options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer( options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = config["JWT:ValidAudience"],
                    ValidateIssuer = true,
                    ValidIssuer = config["JWT:ValidIssuer"],    
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecretKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromDays(double.Parse(config["JWT:TokenLifeTime"]))
                };
            });
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = config["Authentication:Google:ClientId"];
                options.ClientSecret = config["Authentication:Google:ClientSecret"];
            });
        return services;
    }
}