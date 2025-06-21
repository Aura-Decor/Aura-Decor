namespace AuraDecor.APIs.Dtos.Outgoing
{
    public class DeliveryMethodDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
    }
}
