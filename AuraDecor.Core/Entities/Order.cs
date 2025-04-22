using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
