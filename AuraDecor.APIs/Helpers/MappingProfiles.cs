using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.Core.Entities;
using AutoMapper;

namespace AuraDecor.APIs.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Address mappings
        CreateMap<CreateAddressDto, Address>()
            .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.FirstName))
            .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.LastName));
        CreateMap<Address, AddressDto>();

        // Furniture mappings
        CreateMap<AddFurnitureDto, Furniture>()
            .ForMember(dest => dest.PictureUrl, opt => opt.Ignore());

        CreateMap<Furniture, FurnitureToReturnDto>()
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand.Name))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.StyleType, o => o.MapFrom(s => s.StyleType.Name))
            .ForMember(d => d.Color, o => o.MapFrom(s => s.Color.Name));



        // Cart mappings
        CreateMap<Cart, CartDto>()
            .ForMember(d => d.Items, opt => 
                opt.MapFrom(s => s.CartItems));
        
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Furniture.Name))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Furniture.Price))
            .ForMember(d => d.StyleType, opt => opt.MapFrom(s => s.Furniture.StyleType.Name))
            .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Furniture.Color.Name))
            .ForMember(d => d.PictureUrl, opt => opt.MapFrom(s => s.Furniture.PictureUrl));

        CreateMap<CartItem, OrderItem>()
            .ForMember(d => d.FurnitureId, opt => opt.MapFrom(s => s.FurnitureId))
            .ForMember(d => d.Quantity, opt => opt.MapFrom(s => s.Quantity));

        // Order mappings
        CreateMap<Order, OrderToReturnDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.PaymentStatus, opt => opt.MapFrom(s => s.PaymentStatus.ToString()))
            .ForMember(d => d.DeliveryFee, opt => opt.MapFrom(s => s.DeliveryMethod != null ? s.DeliveryMethod.Cost : 0))
            .ForMember(d => d.Subtotal, opt => opt.MapFrom(s => s.OrderAmount - (s.DeliveryMethod != null ? s.DeliveryMethod.Cost : 0)))
            .ForMember(d => d.PaymentIntentId, opt => opt.MapFrom(s => s.PaymentIntentId))
            .ForMember(d => d.ShippingAddress, opt => opt.MapFrom(s => s.ShippingAddress))
            .ForMember(d => d.DeliveryMethod, opt => opt.MapFrom(s => s.DeliveryMethod));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Furniture.Name))
            .ForMember(d => d.PictureUrl, opt => opt.MapFrom(s => s.Furniture.PictureUrl))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Furniture.Price));
            
        CreateMap<DeliveryMethod, DeliveryMethodDto>();
        
        // Notification mappings
        CreateMap<Notification, NotificationDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));

        CreateMap<NotificationPreference, NotificationPreferencesDto>().ReverseMap();
        
        CreateMap<UpdateNotificationPreferencesDto, NotificationPreference>();

        // Rating mappings
        CreateMap<AddRatingDto, Rating>();
        
        CreateMap<Rating, RatingDto>()
            .ForMember(d => d.UserDisplayName, o => o.MapFrom(s => s.User.DisplayName));
            
        // Brand, Category, StyleType, Color mappings
        CreateMap<CreateBrandDto, FurnitureBrand>();
        CreateMap<FurnitureBrand, BrandDto>();
        
        CreateMap<CreateCategoryDto, FurnitureCategory>();
        CreateMap<FurnitureCategory, CategoryDto>();
        
        CreateMap<CreateStyleTypeDto, StyleType>();
        CreateMap<StyleType, StyleTypeDto>();
        
        CreateMap<CreateColorDto, Color>();
        CreateMap<Color, ColorDto>();
    }
}