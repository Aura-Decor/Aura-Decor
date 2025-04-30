using AuraDecor.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Core.Services.Contract
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string UserId , Guid CartId);
        Task<bool> CancelOrderAsync(string UserId, Guid OrderId);
        Task<Order> GetOrderByUserIdAsync(string Id);
    }
}
