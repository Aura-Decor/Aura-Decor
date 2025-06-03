using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AuraDecor.APIs.Dtos.Incoming;

public class AddFurnitureDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(180, MinimumLength = 10)]
    public string Description { get; set; }
    
    [Required]
    public string FurnitureModel { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required]
    public Guid BrandId { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }
    
    [Required]
    public IFormFile Image { get; set; }
}