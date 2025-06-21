using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming;

public class UpdateFurnitureDto
{
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; }
    
    [StringLength(400, MinimumLength = 10)]
    public string? Description { get; set; }
    
    public string? FurnitureModel { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }
    
    public Guid? BrandId { get; set; }
    
    public Guid? CategoryId { get; set; }
    
    public Guid? StyleTypeId { get; set; }
    
    public Guid? ColorId { get; set; }
    
    public IFormFile? Image { get; set; }
}
