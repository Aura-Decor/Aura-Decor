using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Servicies
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddToCartAsync(CartItem cart)
        {
            _unitOfWork.Repository<CartItem>().Add(cart);
            await _unitOfWork.CompleteAsync();

        }

        public Task<Cart> GetCartByUserIdAsync(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveFromCartAsync(CartItem cart)
        {
            _unitOfWork.Repository<CartItem>().DeleteAsync(cart);
            await _unitOfWork.CompleteAsync();

        }
    }
}
