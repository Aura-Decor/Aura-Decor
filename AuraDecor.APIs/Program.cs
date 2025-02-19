#region Using Directives
using AuraDecor.APIs.Extensions;
using AuraDecor.APIs.Middlewares;
using AuraDecor.Repository;
using AuraDecor.Repository.Data;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
#endregion

#region Builder Configuration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSwaggerServices();
#endregion

#region Application Configuration
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseSwaggerMiddleWare(); 

app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
#endregion