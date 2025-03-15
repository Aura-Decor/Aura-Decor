namespace AuraDecor.APIs.Dtos.Outgoing;

public class CartDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public List<CartItemDto> Items { get; set; }

}