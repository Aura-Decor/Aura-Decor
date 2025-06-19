using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.Core.Entities;
using AutoMapper;

namespace AuraDecor.APIs.Helpers;

public class FurniturePicUrlResolver : IValueResolver<Furniture, FurnitureToReturnDto, string>
{
    private readonly IConfiguration _configuration;

    public FurniturePicUrlResolver(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string Resolve(Furniture source, FurnitureToReturnDto destination, string destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.PictureUrl))
        {
            if (source.PictureUrl.StartsWith("http://") || source.PictureUrl.StartsWith("https://"))
            {
                var corsProxy = _configuration["CorsProxy:Url"] ?? "https://corsproxy.io/?url=";
                return $"{corsProxy}{source.PictureUrl}";
            }
            else
            {
                return $"{_configuration["ApiUrls:Base"]}{source.PictureUrl}";
            }
        }
        return null;
    }
}
    
