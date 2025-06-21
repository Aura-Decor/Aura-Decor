using AuraDecor.Core.Entities;
using System;
using System.Linq.Expressions;

namespace AuraDecor.Core.Specifications.OrderSpecification
{
    public class OrdersWithSpecification : BaseSpecification<Order>
    {
        public OrdersWithSpecification(string userId) : base(o => o.UserId == userId)
        {
            ConfigureIncludes();
            AddOrderByDesc(o => o.OrderDate);
        }
        
        public OrdersWithSpecification(Guid orderId) : base(o => o.Id == orderId)
        {
            ConfigureIncludes();
        }
        
        public OrdersWithSpecification(string userId, Guid orderId) : base(o => o.UserId == userId && o.Id == orderId)
        {
            ConfigureIncludes();
        }
        
        private void ConfigureIncludes()
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.OrderItems);
            Includes.Add(o => o.ShippingAddress);
            AddInclude("OrderItems.Furniture");
        }
    }
}
