namespace AuraDecor.APIs.Dtos.Outgoing;

public class CartItemDto
{
    public Guid FurnitureId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; }
}