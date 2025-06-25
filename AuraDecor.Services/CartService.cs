using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications;
using AuraDecor.Core.Specifications.CartSpecification;
using Microsoft.IdentityModel.Tokens;

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
            var cartSpec = new CartByUserIdSpecification(userId);
            var cart = await _unitOfWork.Repository<Cart>().GetWithSpecAsync(cartSpec);
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
               await _unitOfWork.Repository<CartItem>().UpdateAsync(existingItem);
            }
            else
            {
                var furniture = await _unitOfWork.Repository<Furniture>().GetByIdAsync(furnitureId);
                if (furniture == null)
                {
                    throw new Exception("Furniture item not found.");
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
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var spec = new CartWithItemsSpecification(userId);
            return await _unitOfWork.Repository<Cart>().GetWithSpecAsync(spec);
        }

        public async Task RemoveAllItemsFromCartAsync(string userId)
        {
            var spec = new CartByUserIdSpecification(userId);
            var cart = await _unitOfWork.Repository<Cart>().GetWithSpecAsync(spec);
            if (cart == null) throw new Exception("Cart not found!");
            if (!cart.CartItems.Any()) throw new Exception("Empty Cart");
            cart.CartItems.Clear();
            await _unitOfWork.CompleteAsync();
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