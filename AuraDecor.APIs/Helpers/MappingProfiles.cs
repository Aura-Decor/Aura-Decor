using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.Core.Entities;
using AutoMapper;

namespace AuraDecor.APIs.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AddressDto, Address>().ReverseMap();

        CreateMap<Furniture, FurnitureToReturnDto>()
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand.Name))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name));

        CreateMap<Cart, CartDto>()
            .ForMember(d => d.Items, opt => 
                opt.MapFrom(s => s.Items));
        
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Furniture.Name))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Furniture.Price))
            .ForMember(d => d.PictureUrl, opt => opt.MapFrom(s => s.Furniture.PictureUrl));
    }
}