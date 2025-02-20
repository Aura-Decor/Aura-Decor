using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureWithCategoryAndBrandSpec : BaseSpecification<Furniture>
{
    public FurnitureWithCategoryAndBrandSpec(Guid? id = null) : base(id.HasValue ? f => f.Id == id : null)
    {
        Includes.Add(f => f.Brand);
        Includes.Add(f => f.Category);
    }
}