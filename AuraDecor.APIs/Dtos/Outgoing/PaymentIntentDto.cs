using System;

namespace AuraDecor.APIs.Dtos.Outgoing
{
    public class PaymentIntentDto
    {
        public string ClientSecret { get; set; }
        public string PaymentIntentId { get; set; }
        public bool Success { get; set; } = true;
        public string PublishableKey { get; set; }
        public string Message { get; set; } = "Payment intent created successfully";
    }
}