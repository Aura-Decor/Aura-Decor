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
// Use Stripe with alias to avoid conflict
using StripeNamespace = Stripe;

namespace AuraDecor.Servicies
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IPaymentService paymentService,
            ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(string userId, Guid cartId, Address shippingAddress)
        {
            try
            {
                // Validate cart using specification to include CartItems
                var cartSpec = new CartWithItemsSpecification(cartId);
                var cart = await _unitOfWork.Repository<Cart>().GetWithSpecAsync(cartSpec);
                
                if (cart == null || cart.UserId != userId || cart.CartItems == null || !cart.CartItems.Any())
                {
                    _logger.LogError($"Invalid cart: {cartId} for user {userId}. Cart: {cart != null}, UserId match: {cart?.UserId == userId}, Items count: {cart?.CartItems?.Count ?? 0}");
                    throw new Exception("Cart is not valid");
                }
                
                // Validate shipping address
                if (shippingAddress == null || 
                    string.IsNullOrEmpty(shippingAddress.FName) || 
                    string.IsNullOrEmpty(shippingAddress.LName) ||
                    string.IsNullOrEmpty(shippingAddress.Street) ||
                    string.IsNullOrEmpty(shippingAddress.City) ||
                    string.IsNullOrEmpty(shippingAddress.Country))
                {
                    _logger.LogError("Invalid shipping address");
                    throw new Exception("A valid shipping address is required");
                }

                // Set the UserId on the shipping address (required field)
                shippingAddress.UserId = userId;

                // Check if user already has an address (due to unique constraint)
                var existingAddress = await _unitOfWork.Repository<Address>()
                    .FindAsync(a => a.UserId == userId);

                if (existingAddress != null)
                {
                    // Update existing address with new shipping info
                    existingAddress.FName = shippingAddress.FName;
                    existingAddress.LName = shippingAddress.LName;
                    existingAddress.Street = shippingAddress.Street;
                    existingAddress.City = shippingAddress.City;
                    existingAddress.Country = shippingAddress.Country;
                    
                    await _unitOfWork.Repository<Address>().UpdateAsync(existingAddress);
                    await _unitOfWork.CompleteAsync();
                    
                    // Use the existing address for the order
                    shippingAddress = existingAddress;
                }
                else
                {
                    // Create new address if user doesn't have one
                    _unitOfWork.Repository<Address>().Add(shippingAddress);
                    await _unitOfWork.CompleteAsync();
                }

                // Create or update the payment intent
                var paymentIntent = await _paymentService.CreateOrUpdatePaymentIntentAysnc(cartId);
                _logger.LogInformation($"Payment intent {paymentIntent.Id} created/updated for cart {cartId}");

                // Calculate total order amount
                decimal total = cart.CartItems.Sum(item => item.Quantity * item.Furniture.Price);
                
                // Add delivery cost if delivery method is selected
                if (cart.DeliveryMethodId.HasValue)
                {
                    var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(cart.DeliveryMethodId.Value);
                    if (deliveryMethod != null)
                    {
                        total += deliveryMethod.Cost;
                    }
                }

                // Create the order
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

                // Add the order first
                _unitOfWork.Repository<Order>().Add(order);
                await _unitOfWork.CompleteAsync();

                // Create order items from cart items
                var orderItems = cart.CartItems.Select(cartItem => new OrderItem
                {
                    FurnitureId = cartItem.FurnitureId,
                    Quantity = cartItem.Quantity,
                    Furniture = cartItem.Furniture
                }).ToList();

                // Add the order items to the order
                order.OrderItems = orderItems;

                // Save the order items
                await _unitOfWork.CompleteAsync();
                
                _logger.LogInformation($"Order created successfully: {order.Id} for user {userId}");
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating order: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(string userId, Guid orderId)
        {
            try
            {
                // Get the order with a specification to include all related data
                var spec = new OrdersWithSpecification(userId, orderId);
                var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);

                if (order == null)
                {
                    _logger.LogWarning($"Order not found: {orderId}");
                    return false;
                }
                
                if (order.UserId != userId)
                {
                    _logger.LogWarning($"User {userId} attempted to cancel order {orderId} belonging to another user");
                    return false;
                }

                if (order.Status == OrderStatus.Cancelled)
                {
                    _logger.LogInformation($"Order {orderId} is already cancelled");
                    return false;
                }

                // Don't allow cancellation if payment has already been processed
                if (order.PaymentStatus == PaymentStatus.Succeeded)
                {
                    _logger.LogWarning($"Cannot cancel order {orderId} as payment already succeeded");
                    return false;
                }

                // Try to cancel the Stripe payment intent if it exists
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
                        _logger.LogError(ex, $"Failed to cancel Stripe payment intent {order.PaymentIntentId} for order {orderId}, continuing with order cancellation");
                        // Continue with order cancellation even if we can't cancel the payment intent
                    }
                }

                // Update order status
                order.Status = OrderStatus.Cancelled;
                order.PaymentStatus = PaymentStatus.Failed;
                
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Order {orderId} cancelled successfully by user {userId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {orderId} for user {userId}");
                return false;
            }
        }

        public async Task<Order> GetOrderByUserIdAsync(string id)
        {
            try
            {
                var spec = new OrdersWithSpecification(id);
                return await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order for user {id}");
                throw;
            }
        }
        
        public async Task<Order> GetOrderByIdAsync(Guid orderId, string userId = null)
        {
            try
            {
                if (userId != null)
                {
                    var spec = new OrdersWithSpecification(userId, orderId);
                    return await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
                }
                else
                {
                    // Admin access - get order regardless of user
                    var spec = new OrdersWithSpecification(orderId);
                    return await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order {orderId}");
                throw;
            }
        }
        
        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string userId)
        {
            try
            {
                var spec = new OrdersWithSpecification(userId);
                return await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for user {userId}");
                throw;
            }
        }
    }
}
