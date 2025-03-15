using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.CartSpecification
{
    public class CartItemsWithSpecification : BaseSpecification<Cart> 
    { 
        public CartItemsWithSpecification (string userId) : base(u => u.UserId == userId)
        {
            Includes.Add(u => u.Items);
        }
    }
}
