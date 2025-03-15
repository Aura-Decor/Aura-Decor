using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using  AuraDecor.Core.Specifications.CartSpecification;
//using AuraDecor.APIs.Errors;


namespace AuraDecor.Servicies
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddToCartAsync(string userId, Guid furnitureId, int quantity)
        {
            var Cartspec = new CartByUserIdSpecification(userId);
            var cart = await _unitOfWork.Repository<Cart>().GetWithSpecAsync(Cartspec);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                 _unitOfWork.Repository<Cart>().Add(cart);
                await _unitOfWork.CompleteAsync();
            }
            var spec = new CartItemSpecification(cart.Id, furnitureId);
            var existingItem = await _unitOfWork.Repository<CartItem>().GetWithSpecAsync(spec);


            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _unitOfWork.Repository<CartItem>().UpdateAsync(existingItem);
            }
            else
            {
                var furniture = await _unitOfWork.Repository<Furniture>().GetByIdAsync(furnitureId);
                if (furniture == null)
                {
                 //   return new ApiResponse(404, "Furniture item not found.");
                }

                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    FurnitureId = furnitureId,
                    Quantity = quantity
                };

                 _unitOfWork.Repository<CartItem>().Add(cartItem);
            }

            await _unitOfWork.CompleteAsync();
          //  return new ApiResponse(200, "Item added to cart successfully.");


        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var spec = new CartItemsWithSpecification(userId);
            return await _unitOfWork.Repository<Cart>().GetWithSpecAsync(spec);

        }

        public async Task RemoveFromCartAsync(string userId, Guid furnitureId)
        {

            var spec = new CartByUserIdSpecification(userId);
            var cart = await _unitOfWork.Repository<Cart>().GetWithSpecAsync(spec);

            if (cart == null)
                throw new Exception("Cart not found");

            var specItem = new CartItemSpecification(cart.Id, furnitureId);
            var cartItem = await _unitOfWork.Repository<CartItem>().GetWithSpecAsync(specItem);

            if (cartItem == null)
                throw new Exception("Item not found in cart");

            _unitOfWork.Repository<CartItem>().DeleteAsync(cartItem);

            await _unitOfWork.CompleteAsync();
        }
    }
}
