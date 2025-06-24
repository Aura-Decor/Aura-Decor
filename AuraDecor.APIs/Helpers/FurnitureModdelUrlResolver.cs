using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.Core.Entities;
using AutoMapper;

namespace AuraDecor.APIs.Helpers;

public class FurnitureModdelUrlResolver : IValueResolver<Furniture, FurnitureToReturnDto, string>
{
    private readonly IConfiguration _configuration;

    public FurnitureModdelUrlResolver(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string Resolve(Furniture source, FurnitureToReturnDto destination, string destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.FurnitureModel))
        {
            if (source.FurnitureModel.StartsWith("http://") || source.FurnitureModel.StartsWith("https://"))
            {
                var corsProxy = _configuration["CorsProxy:Url"] ?? "https://corsproxy.io/?url=";
                return $"{corsProxy}{source.FurnitureModel}";
            }
            else
            {
                return $"{_configuration["ApiUrls:Base"]}{source.FurnitureModel}";
            }
        }
        return null;
    }
}
    
