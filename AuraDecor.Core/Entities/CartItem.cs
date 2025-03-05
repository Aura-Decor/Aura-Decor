using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Core.Entities
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; set; }
        public Guid CartId { get; set; } 
        public Cart Cart { get; set; } // Navigation property one to many
        public Guid FurnitureId { get;set; }
        public Furniture Furniture { get; set; } // Navigation property many to one
        private decimal _totalPrice;

        [NotMapped] 
        public decimal TotalPrice 
        { 
            get => Quantity * (Furniture?.Price ?? 0);
            private set => _totalPrice = value;
        }
        
    }
}
