using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.Core.Entities;
using AutoMapper;

namespace AuraDecor.APIs.Helpers;

public class CartItemPicUrlResolver : IValueResolver<CartItem, CartItemDto, string>
{
    private readonly IConfiguration _configuration;

    public CartItemPicUrlResolver(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Resolve(CartItem source, CartItemDto destination, string destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.Furniture?.PictureUrl))
        {
            if (source.Furniture.PictureUrl.StartsWith("http://") || source.Furniture.PictureUrl.StartsWith("https://"))
            {
                var corsProxy = _configuration["CorsProxy:Url"] ?? "https://corsproxy.io/?url=";
                return $"{corsProxy}{source.Furniture.PictureUrl}";
            }
            else
            {
                return $"{_configuration["ApiUrls:Base"]}{source.Furniture.PictureUrl}";
            }
        }
        return null;
    }
}