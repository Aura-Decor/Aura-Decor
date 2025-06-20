using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Servicies
{
    public class PaymentService : IPaymentService
    {
        public readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public PaymentService(IUnitOfWork unitOfWork , IMapper mapper, IConfiguration config)
        {
             _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

        }

        public async Task<PaymentIntent> CreateOrUpdatePaymentIntentAysnc(Guid CartId)
        {
            var cart = await _unitOfWork.Repository<Cart>().GetByIdAsync(CartId);

            if (cart == null)
            {
                throw new Exception("Cart not found");

            }
            if (!cart.CartItems.Any())
            {
                throw new Exception("Empty cart");
            }

            decimal deliveryCost = 0;

            if (cart.DeliveryMethodId.HasValue)
            {
                var delivery = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(cart.DeliveryMethodId.Value);
                if (delivery != null)
                {
                    deliveryCost = delivery.Cost;
                }

            }
            var amount = cart.CartItems.Sum(i => i.Quantity * i.Furniture.Price) + deliveryCost;

            long amountInCents = (long)(amount * 100);

            var service = new PaymentIntentService();
            PaymentIntent intent;


            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var createOptions = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                };

                intent = await service.CreateAsync(createOptions);

                cart.PaymentIntentId = intent.Id;
                await _unitOfWork.Repository<Cart>().UpdateAsync(cart);
            }
            else
            {
                var updateOptions = new PaymentIntentUpdateOptions
                {
                    Amount = amountInCents
                };

                intent = await service.UpdateAsync(cart.PaymentIntentId, updateOptions);
            }

            return intent;
        }
    }
}
