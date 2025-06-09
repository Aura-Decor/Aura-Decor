using AuraDecor.Core.Entities;
using System;
using System.Linq.Expressions;

namespace AuraDecor.Core.Specifications
{
    public class RatingByUserAndProductSpecification : BaseSpecification<Rating>
    {
        public RatingByUserAndProductSpecification(string userId, Guid productId)
            : base(r => r.UserId == userId && r.ProductId == productId)
        {
        }
    }
} 