using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureWithCategoryAndBrandSpec : BaseSpecification<Furniture>
{
    public FurnitureWithCategoryAndBrandSpec(FurnitureSpecParams specParams)
        : base(p =>
            (string.IsNullOrEmpty(specParams.search) || p.Name.ToLower().Contains(specParams.search)) &&
            (!specParams.brandId.HasValue || p.BrandId == specParams.brandId) &&
            (!specParams.categoryId.HasValue || p.CategoryId == specParams.categoryId))
    {
        Includes.Add(p => p.Brand);
        Includes.Add(p => p.Category);
        switch (specParams.sort)
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
       
        ApplyPagingation((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
    }

    public FurnitureWithCategoryAndBrandSpec(Guid id)
        : base(p => p.Id == id)
    {
        Includes.Add(p => p.Brand);
        Includes.Add(p => p.Category);
    }

 
}
