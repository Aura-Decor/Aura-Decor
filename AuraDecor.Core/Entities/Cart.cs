using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Core.Entities
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }
        public ICollection<CartItem> cartItems { get; set; } = new List<CartItem>();
    }
}
