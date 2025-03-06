using AuraDecor.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Core.Services.Contract
{
    public interface ICartService
    {


        Task<Cart> GetCartByUserIdAsync(Guid Id);
        Task AddToCartAsync(CartItem cart);
        Task RemoveFromCartAsync(CartItem cart);
    }
}
