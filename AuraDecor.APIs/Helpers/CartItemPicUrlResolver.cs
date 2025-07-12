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
      
                return $"{_configuration["ApiUrls:Base"]}{source.Furniture.PictureUrl}";
        }
        return null;
    }
    
}