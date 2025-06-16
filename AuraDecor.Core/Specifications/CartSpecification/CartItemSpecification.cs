using AuraDecor.Core.Entities;


namespace AuraDecor.Core.Specifications.CartSpecification
{
   public class CartItemSpecification : BaseSpecification<CartItem>
    {
        public CartItemSpecification(Guid cartId, Guid furnitureId)
            : base(c => c.CartId == cartId && c.FurnitureId == furnitureId)
        {
            Includes.Add(c => c.Furniture);
            Includes.Add(c => c.Furniture.StyleType);
            Includes.Add(c => c.Furniture.Color);
            Includes.Add(c => c.Furniture.Brand);
            Includes.Add(c => c.Furniture.Category);
        }
    
    }
}
