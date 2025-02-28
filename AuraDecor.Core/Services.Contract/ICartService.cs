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
      //  Task AddItemAsync(CartItem cartItem, int quantity);
        Task DeleteItemAsync(Guid cartItemId);
        Task UpdateItemAsync(Guid cartItemId, int quantity);
        Task DeleteAllAsync();

        Task<IReadOnlyList<CartItem>> GetAllItemsAsync();

    }
}
