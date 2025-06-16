namespace AuraDecor.APIs.Dtos.Outgoing;

public class FurnitureToReturnDto
{
    public Guid Id { get; set; } 
    public string Name { get; set; }
    public string Description { get; set; }
    public string PictureUrl { get; set; }
    public string FurnitureModel { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public bool HasOffer { get; set; }
    public DateTime? OfferStartDate { get; set; }
    public DateTime? OfferEndDate { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public Guid BrandId { get; set; }
    public string Brand { get; set; } 
    public Guid CategoryId { get; set; }
    public string Category { get; set; } 
    public Guid StyleTypeId { get; set; }
    public string StyleType { get; set; }
    public Guid ColorId { get; set; }
    public string Color { get; set; }
}