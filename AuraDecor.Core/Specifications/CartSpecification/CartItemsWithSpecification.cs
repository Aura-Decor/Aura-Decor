using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications;

public class CartWithItemsSpecification : BaseSpecification<Cart>
{
    public CartWithItemsSpecification(string userId)
        : base(cart => cart.UserId == userId)
    {
        AddInclude("CartItems.Furniture");
        AddInclude("CartItems.Furniture.StyleType");
        AddInclude("CartItems.Furniture.Color");
        AddInclude("CartItems.Furniture.Brand");
        AddInclude("CartItems.Furniture.Category");
    }
    
    public CartWithItemsSpecification(Guid cartId)
        : base(cart => cart.Id == cartId)
    {
        AddInclude("CartItems.Furniture");
        AddInclude("CartItems.Furniture.StyleType");
        AddInclude("CartItems.Furniture.Color");
        AddInclude("CartItems.Furniture.Brand");
        AddInclude("CartItems.Furniture.Category");
    }
}