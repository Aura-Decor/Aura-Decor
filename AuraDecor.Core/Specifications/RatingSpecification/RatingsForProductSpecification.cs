using AuraDecor.Core.Entities;
using System;
using System.Linq.Expressions;

namespace AuraDecor.Core.Specifications
{
    public class RatingsForProductSpecification : BaseSpecification<Rating>
    {
        public RatingsForProductSpecification(Guid productId)
            : base(r => r.ProductId == productId)
        {
            AddInclude("User");
            AddOrderByDesc(r => r.CreatedAt);
        }
    }
} 