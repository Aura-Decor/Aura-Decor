#region Using Directives
using AuraDecor.APIs.Extensions;
using AuraDecor.APIs.Middlewares;
using AuraDecor.Repository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Cors; 
#endregion

#region Builder Configuration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSwaggerServices();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
#endregion

#region Application Configuration

var app = builder.Build();
#region DatabaseMigration
using var scope = app.Services.CreateScope();
          
// Ask CLR to create a scope for the service provider
var services = scope.ServiceProvider;
var _dbcontext = services.GetRequiredService<AppDbContext>();
var loggerFactory = services.GetRequiredService<ILoggerFactory>();
try
{
    await _dbcontext.Database.MigrateAsync();
    await AppDbContextDataSeed.SeedAsync(_dbcontext);
                
}
catch (Exception e)
{
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(e, "An error occurred during migration");
}
#endregion

app.UseMiddleware<ExceptionMiddleware>();
app.UseSwaggerMiddleWare(); 
app.MapScalarApiReference(options => 
    options
        .WithTheme(ScalarTheme.Mars)
        .WithDefaultHttpClient(ScalarTarget.CSharp,ScalarClient.HttpClient)
                    
);
app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll"); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
#endregion