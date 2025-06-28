using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications.OrderSpecification;
using AuraDecor.Core.Specifications;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDecor.Core.Entities.Enums;
using StripeNamespace = Stripe;

namespace AuraDecor.Servicies
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<OrderService> _logger;
        private readonly ICartService _cartService;

        public OrderService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            IPaymentService paymentService,
            ILogger<OrderService> logger,
            ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentService = paymentService;
            _logger = logger;
            _cartService = cartService;
        }

        public async Task<(Order order, PaymentIntentResponse paymentIntent)> CreateOrderAsync(string userId, Guid cartId, Address shippingAddress)
        {
            var cartSpec = new CartWithItemsSpecification(cartId);
            var cart = await _unitOfWork.Repository<Cart>().GetWithSpecAsync(cartSpec);
            
            if (cart == null || cart.UserId != userId || cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new Exception("Cart is not valid");
            }
            
            if (shippingAddress == null || 
                string.IsNullOrEmpty(shippingAddress.FirstName) || 
                string.IsNullOrEmpty(shippingAddress.LastName) ||
                string.IsNullOrEmpty(shippingAddress.Street) ||
                string.IsNullOrEmpty(shippingAddress.City) ||
                string.IsNullOrEmpty(shippingAddress.Country))
            {
                throw new Exception("A valid shipping address is required");
            }

                shippingAddress.UserId = userId;

                var existingAddress = await _unitOfWork.Repository<Address>()
                    .FindAsync(a => a.UserId == userId);

                if (existingAddress != null)
                {
                    existingAddress.FirstName = shippingAddress.FirstName;
                    existingAddress.LastName = shippingAddress.LastName;
                    existingAddress.Street = shippingAddress.Street;
                    existingAddress.City = shippingAddress.City;
                    existingAddress.State = shippingAddress.State;
                    existingAddress.ZipCode = shippingAddress.ZipCode;
                    existingAddress.Country = shippingAddress.Country;
                    existingAddress.PhoneNumber = shippingAddress.PhoneNumber;
                    existingAddress.DeliveryInstructions = shippingAddress.DeliveryInstructions;
                    
                    await _unitOfWork.Repository<Address>().UpdateAsync(existingAddress);
                    await _unitOfWork.CompleteAsync();
                    
                    shippingAddress = existingAddress;
                }
                else
                {
                    _unitOfWork.Repository<Address>().Add(shippingAddress);
                    await _unitOfWork.CompleteAsync();
                }

                var paymentIntent = await _paymentService.CreateOrUpdatePaymentIntentAysnc(cartId);

                decimal total = cart.CartItems.Sum(item => item.Quantity * item.Furniture.Price);
                
                if (cart.DeliveryMethodId.HasValue)
                {
                    var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(cart.DeliveryMethodId.Value);
                    if (deliveryMethod != null)
                    {
                        total += deliveryMethod.Cost;
                    }
                }

                var order = new Order
                {
                    UserId = userId,
                    CartId = cartId,
                    OrderDate = DateTime.UtcNow,
                    OrderAmount = total,
                    Status = OrderStatus.Pending,
                    PaymentStatus = PaymentStatus.Pending,
                    PaymentIntentId = paymentIntent.Id,
                    ShippingAddress = shippingAddress,
                    DeliveryMethodId = cart.DeliveryMethodId
                };

                _unitOfWork.Repository<Order>().Add(order);
                await _unitOfWork.CompleteAsync();

                var orderItems = cart.CartItems.Select(cartItem => new OrderItem
                {
                    FurnitureId = cartItem.FurnitureId,
                    Quantity = cartItem.Quantity,
                    Furniture = cartItem.Furniture
                }).ToList();

                order.OrderItems = orderItems;

                await _unitOfWork.CompleteAsync();
                
                _logger.LogInformation($"Order created successfully: {order.Id} for user {userId}");
                
                // Create payment response
                var paymentResponse = new PaymentIntentResponse
                {
                    ClientSecret = paymentIntent.ClientSecret,
                    PaymentIntentId = paymentIntent.Id
                };
            await _cartService.RemoveAllItemsFromCartAsync(userId);
                return (order, paymentResponse);
        }

        public async Task<bool> CancelOrderAsync(string userId, Guid orderId)
        {
            var spec = new OrdersWithSpecification(userId, orderId);
            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);

            if (order == null)
            {
                return false;
            }
            
            if (order.UserId != userId)
            {
                return false;
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                return false;
            }

            if (order.PaymentStatus == PaymentStatus.Succeeded)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(order.PaymentIntentId))
            {
                try
                {
                    var service = new Stripe.PaymentIntentService();
                    await service.CancelAsync(order.PaymentIntentId);
                    _logger.LogInformation($"Stripe payment intent {order.PaymentIntentId} cancelled for order {orderId}");
                }
                catch (Stripe.StripeException ex)
                {
                    _logger.LogWarning($"Failed to cancel Stripe payment intent {order.PaymentIntentId} for order {orderId}: {ex.Message}");
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.PaymentStatus = PaymentStatus.Failed;
            
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"Order {orderId} cancelled successfully by user {userId}");

            return true;
        }

        public async Task<Order> GetOrderByUserIdAsync(string id)
        {
            var spec = new OrdersWithSpecification(id);
            return await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
        }
        
        public async Task<Order> GetOrderByIdAsync(Guid orderId, string userId = null)
        {
            if (userId != null)
            {
                var spec = new OrdersWithSpecification(userId, orderId);
                return await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            }
            else
            {
                var spec = new OrdersWithSpecification(orderId);
                return await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            }
        }
        
        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string userId)
        {
            var spec = new OrdersWithSpecification(userId);
            return await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
        }
    }
}
