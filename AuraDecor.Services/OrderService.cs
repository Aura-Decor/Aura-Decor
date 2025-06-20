﻿using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications.OrderSpecification;
using AutoMapper;
using Org.BouncyCastle.Crypto;
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

        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentService = paymentService;
        }


        public async Task<Order> CreateOrderAsync(string userId, Guid cartId)
        {
            var cart = await _unitOfWork.Repository<Cart>().GetByIdAsync(cartId);

            if (cart == null || cart.UserId != userId || !cart.CartItems.Any())
                throw new Exception("cart is not valid");

            var paymentIntent = await _paymentService.CreateOrUpdatePaymentIntentAysnc(cartId);

            var orderItems = _mapper.Map<List<OrderItem>>(cart.CartItems);

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderItems = orderItems
            };

             _unitOfWork.Repository<Order>().Add(order);
            await _unitOfWork.CompleteAsync();

            return order;
        }

        public async Task<bool> CancelOrderAsync(string UserId, Guid OrderId)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(OrderId);

            if (order == null || order.UserId != UserId)
                return false;

            if (order.Status == OrderStatus.Cancelled)
                return false;

            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<Order> GetOrderByUserIdAsync(string Id)
        {
            var spec = new OrdersWithSpecification(Id);
            return await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
        }
    }
}
