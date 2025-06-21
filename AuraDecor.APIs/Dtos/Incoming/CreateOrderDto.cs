using System;
using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming
{
    public class CreateOrderDto
    {
        [Required]
        public Guid CartId { get; set; }
        
        public CreateAddressDto ShippingAddress { get; set; }
        
        public Guid? DeliveryMethodId { get; set; }
    }
}