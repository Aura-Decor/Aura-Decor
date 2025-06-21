using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Core.Services.Contract
{
    public interface IOrderService
    {
        Task<(Order order, PaymentIntentResponse paymentIntent)> CreateOrderAsync(string userId, Guid cartId, Address shippingAddress);
        Task<bool> CancelOrderAsync(string userId, Guid orderId);
        Task<Order> GetOrderByIdAsync(Guid orderId, string userId = null);
        Task<IEnumerable<Order>> GetOrdersForUserAsync(string userId);
    }
}
