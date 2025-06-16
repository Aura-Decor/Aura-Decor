using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureWithCategoryAndBrandAndStyleTypeAndColorSpec : BaseSpecification<Furniture>
{
    public FurnitureWithCategoryAndBrandAndStyleTypeAndColorSpec(FurnitureSpecParams specParams)
        : base(p =>
            (string.IsNullOrEmpty(specParams.search) || p.Name.ToLower().Contains(specParams.search)) &&
            (!specParams.brandId.HasValue || p.BrandId == specParams.brandId) &&
            (!specParams.categoryId.HasValue || p.CategoryId == specParams.categoryId) &&
            (!specParams.StyleTypeId.HasValue ||p.StyleTypeId == specParams.StyleTypeId) &&
            (!specParams.ColorId.HasValue || p.ColorId == specParams.ColorId)) 
        {
        Includes.Add(p => p.Brand);
        Includes.Add(p => p.Category);
        Includes.Add(p => p.StyleType);
        Includes.Add(p => p.Color);

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

    public FurnitureWithCategoryAndBrandAndStyleTypeAndColorSpec(Guid id)
        : base(p => p.Id == id)
    {
        Includes.Add(p => p.Brand);
        Includes.Add(p => p.Category);
        Includes.Add(p => p.StyleType);
        Includes.Add(p => p.Color);
    }

 
}
