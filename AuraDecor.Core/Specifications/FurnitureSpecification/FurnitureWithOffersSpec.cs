using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureWithOffersSpec : BaseSpecification<Furniture>
{
    public FurnitureWithOffersSpec()
        : base(p => p.DiscountPercentage.HasValue || 
                    p.OfferStartDate.HasValue || 
                    p.OfferEndDate.HasValue)
    {
        AddIncludes();
    }

    private void AddIncludes()
    {
        Includes.Add(p => p.Brand);
        Includes.Add(p => p.Category);
    }
}