using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Repository;
using AuraDecor.Repository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();
using var scope = app.Services.CreateScope();
          
#region DatabaseMigration
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();


app.Run();


