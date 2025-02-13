#region Using Directives
using AuraDecor.APIs.Extensions;
using AuraDecor.APIs.Middlewares;
using AuraDecor.Repository;
using AuraDecor.Repository.Data;
#endregion

#region Builder Configuration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);
#endregion

#region Application Configuration
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

app.Run();
#endregion