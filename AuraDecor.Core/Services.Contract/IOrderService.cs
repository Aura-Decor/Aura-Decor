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
        Task<Order> CreateOrderAysnc(string UserId , Guid CartId);
        Task<bool> CancelOrderAysnc(string UserId, Guid OrderId);
        Task<Order> GetOrderByUserIdAysnc(string Id);
    }
}
