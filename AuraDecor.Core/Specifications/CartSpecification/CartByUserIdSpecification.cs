using AuraDecor.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Core.Specifications.CartSpecification
{
    public class CartByUserIdSpecification : BaseSpecification<Cart>
    {
        public CartByUserIdSpecification(string userId) : base(c => c.UserId == userId)
        {
            Includes.Add(c => c.CartItems);

        }
    }
}
