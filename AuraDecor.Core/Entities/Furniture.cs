using System.ComponentModel.DataAnnotations;

namespace AuraDecor.Core.Entities;

public class Furniture : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string PictureUrl { get; set; }
    public string FurnitureModel { get; set; }
    public decimal Price { get; set; }
    public Guid BrandId { get; set; }
    public FurnitureBrand Brand { get; set; } // Navigation property one to many 
    public Guid CategoryId { get; set; }
    public FurnitureCategory Category { get; set; } // Navigation property one to many
}