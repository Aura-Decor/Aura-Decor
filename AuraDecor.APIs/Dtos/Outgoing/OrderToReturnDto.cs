namespace AuraDecor.APIs.Dtos.Outgoing
{
    public class OrderToReturnDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public string? PaymentIntentId { get; set; }
        public AddressDto ShippingAddress { get; set; }
        public DeliveryMethodDto? DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderCreationResponseDto
    {
        public OrderToReturnDto Order { get; set; }
        public PaymentIntentDto? PaymentIntent { get; set; }
    }
}