namespace AuraDecor.APIs.Dtos.Incoming;

public class AddToCartDto
{
    public Guid FurnitureId { get; set; }
    public int Quantity { get; set; }
}