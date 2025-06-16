using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureWithActiveOffersSpec : BaseSpecification<Furniture>
{
    public FurnitureWithActiveOffersSpec()
        : base(p => p.HasOffer && 
                    p.OfferStartDate <= DateTime.UtcNow && 
                    p.OfferEndDate >= DateTime.UtcNow)
    {
        AddIncludes();
        AddOrderBy(p => p.Name);
    }

    private void AddIncludes()
    {
        Includes.Add(p => p.Brand);
        Includes.Add(p => p.Category);
        Includes.Add(p => p.StyleType);
        Includes.Add(p => p.Color);
    }
}