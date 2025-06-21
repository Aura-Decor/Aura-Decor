using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDecor.Core.Entities.Enums;

namespace AuraDecor.Core.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }
        public DateTime OrderDate { get; set; }
        public Decimal OrderAmount { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public string? PaymentIntentId { get; set; }
        public Address ShippingAddress { get; set; }
        public Guid? DeliveryMethodId { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}
