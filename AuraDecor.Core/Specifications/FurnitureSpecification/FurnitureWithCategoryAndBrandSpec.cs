using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureWithCategoryAndBrandSpec : BaseSpecification<Furniture>
{

    public FurnitureWithCategoryAndBrandSpec(Guid? id) : base(f=>f.Id == id)
    {
        Includes.Add(F=>F.Brand);
        Includes.Add(F=>F.Category);
    }
    public FurnitureWithCategoryAndBrandSpec() : base()
    {
        Includes.Add(F=>F.Brand);
        Includes.Add(F=>F.Category);
    }
    
}