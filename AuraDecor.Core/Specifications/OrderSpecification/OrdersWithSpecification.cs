using AuraDecor.Core.Entities;


namespace AuraDecor.Core.Specifications.OrderSpecification
{
    public class OrdersWithSpecification:BaseSpecification<Order>
    {
        public OrdersWithSpecification(string Id) : base(o => o.UserId == Id)
        {
                    
        }
    }
}
