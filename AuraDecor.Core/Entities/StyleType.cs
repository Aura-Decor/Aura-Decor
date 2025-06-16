
namespace AuraDecor.Core.Entities
{
    public class StyleType : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Furniture> Furnitures { get; set; }
    }
}
