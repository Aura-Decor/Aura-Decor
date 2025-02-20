using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureWithCategoryAndBrandSpec : BaseSpecification<Furniture>
{
    public FurnitureWithCategoryAndBrandSpec(string sort, Guid? brandId, Guid? categoryId)
        : base(p =>
            (!brandId.HasValue || p.BrandId == brandId) &&
            (!categoryId.HasValue || p.CategoryId == categoryId))
    {
        AddIncludes();
        switch (sort)
        {
            case "priceAsc":
                AddOrderBy(p => p.Price);
                break;
            case "priceDesc":
                AddOrderByDesc(p => p.Price);
                break;
            default:
                AddOrderBy(p => p.Name);
                break;
        }
    }

    public FurnitureWithCategoryAndBrandSpec(Guid id)
        : base(p => p.Id == id)
    {
        AddIncludes();
    }

    private void AddIncludes()
    {
        Includes.Add(p => p.Brand);
        Includes.Add(p => p.Category);
    }
}
