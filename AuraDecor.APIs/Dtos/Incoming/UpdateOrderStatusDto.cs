using System;
using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [Required]
        [RegularExpression("^(Pending|Processing|Completed|Cancelled)$",
            ErrorMessage = "Status must be one of: Pending, Processing, Completed, Cancelled")]
        public string Status { get; set; }
        
        // Optional notes about the status update
        public string Notes { get; set; }
    }
}
