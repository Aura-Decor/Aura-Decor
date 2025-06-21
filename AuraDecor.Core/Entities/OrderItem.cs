using System.ComponentModel.DataAnnotations.Schema;

namespace AuraDecor.Core.Entities
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public Guid FurnitureId { get; set; }
        public Furniture Furniture { get; set; }

        private decimal _totalPrice;

        [NotMapped]
        public decimal TotalPrice
        {
            get => Quantity * (Furniture?.Price ?? 0);
            private set => _totalPrice = value;
        }
    }
}
