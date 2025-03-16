using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications;

public class CartWithItemsSpecification : BaseSpecification<Cart>
{
    public CartWithItemsSpecification(string userId) 
        : base(cart => cart.UserId == userId)
    {
    }
}