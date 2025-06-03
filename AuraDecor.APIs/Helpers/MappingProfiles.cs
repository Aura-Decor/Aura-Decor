using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.Core.Entities;
using AutoMapper;

namespace AuraDecor.APIs.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AddressDto, Address>().ReverseMap();

        CreateMap<AddFurnitureDto, Furniture>()
            .ForMember(dest => dest.PictureUrl, opt => opt.Ignore());

        CreateMap<Furniture, FurnitureToReturnDto>()
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand.Name))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name));

        CreateMap<Cart, CartDto>()
            .ForMember(d => d.Items, opt => 
                opt.MapFrom(s => s.CartItems));
        
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Furniture.Name))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Furniture.Price))
            .ForMember(d => d.PictureUrl, opt => opt.MapFrom(s => s.Furniture.PictureUrl));

        CreateMap<CartItem, OrderItem>()
            .ForMember(d => d.FurnitureId, opt => opt.MapFrom(s => s.FurnitureId))
            .ForMember(d => d.CartId, opt => opt.MapFrom(s => s.CartId))
            .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Quantity));

        // Notification mappings
        CreateMap<Notification, NotificationDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));

        CreateMap<NotificationPreference, NotificationPreferencesDto>().ReverseMap();
        
        CreateMap<UpdateNotificationPreferencesDto, NotificationPreference>();
    }
}