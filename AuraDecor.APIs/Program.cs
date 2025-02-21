#region Using Directives
using AuraDecor.APIs.Extensions;
using AuraDecor.APIs.Middlewares;
using Scalar.AspNetCore;



#endregion

#region Builder Configuration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSwaggerServices();
#endregion

#region Application Configuration
var app = builder.Build();

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
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
#endregion