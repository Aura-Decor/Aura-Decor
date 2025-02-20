namespace AuraDecor.APIs.Dtos.Outgoing;

public class FurnitureToReturnDto
{
    public Guid Id { get; set; } 
    public string Name { get; set; }
    public string Description { get; set; }
    public string PictureUrl { get; set; }
    public string FurnitureModel { get; set; }
    public decimal Price { get; set; }
    public Guid BrandId { get; set; }
    public string Brand { get; set; } 
    public Guid CategoryId { get; set; }
    public string Category { get; set; } 
}