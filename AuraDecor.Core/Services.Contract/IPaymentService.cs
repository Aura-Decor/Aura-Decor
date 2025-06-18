using Stripe;

namespace AuraDecor.Core.Services.Contract;

public interface IPaymentService
{

    Task<PaymentIntent> CreateOrUpdatePaymentIntentAysnc(Guid CartId);

}