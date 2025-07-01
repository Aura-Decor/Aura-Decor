namespace AuraDecor.APIs.Dtos.Outgoing
{
    public class ImageSearchResultDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public string FurnitureModel { get; set; }
        public decimal Price { get; set; }
        public bool HasOffer { get; set; }
        public DateTime? OfferStartDate { get; set; }
        public DateTime? OfferEndDate { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string BrandId { get; set; }
        public string CategoryId { get; set; }
        public string StyleTypeId { get; set; }
        public string ColorId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
