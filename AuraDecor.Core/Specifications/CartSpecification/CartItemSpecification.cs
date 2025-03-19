using AuraDecor.Core.Entities;


namespace AuraDecor.Core.Specifications.CartSpecification
{
   public class CartItemSpecification : BaseSpecification<CartItem>
    {
        public CartItemSpecification(Guid cartId, Guid furnitureId)
            : base(c => c.CartId == cartId && c.FurnitureId == furnitureId)
        {
            Includes.Add(c => c.Furniture);
        }
    
    }
}
