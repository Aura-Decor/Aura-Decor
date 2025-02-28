using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Core.Entities
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; set; }
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }
        public Guid FurnitureId { get;set; }
        public Furniture Furniture { get; set; }
        public decimal TotalPrice => Quantity * Furniture.Price; 
    }
}
