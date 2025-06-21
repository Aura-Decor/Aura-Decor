namespace AuraDecor.APIs.Dtos.Outgoing
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
