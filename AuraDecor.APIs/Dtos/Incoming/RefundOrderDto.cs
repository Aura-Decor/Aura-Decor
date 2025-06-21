using System;
using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming
{
    public class RefundOrderDto
    {
        [Required]
        public Guid OrderId { get; set; }
        
        // Optional: If not provided, refunds the full amount
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        // Optional: For tracking refund reason
        public string Reason { get; set; }
    }
}