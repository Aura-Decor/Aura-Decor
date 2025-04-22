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
    public class OrderService : IOrderService
    {
        public readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
                _unitOfWork = unitOfWork;
        }

        public Task<Order> CreateOrderAysnc(string UserId, Guid CartId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CancelOrderAysnc(string UserId, Guid OrderId)
        {
            throw new NotImplementedException();
        }
    }
}
