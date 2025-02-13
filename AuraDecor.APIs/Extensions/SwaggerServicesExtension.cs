namespace AuraDecor.APIs.Extensions;

public static class SwaggerServicesExtension
{
    public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(); 

        return services;
    }
}