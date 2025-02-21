using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureWithFiltrationForCountSpec : BaseSpecification<Furniture>
{

    public FurnitureWithFiltrationForCountSpec(FurnitureSpecParams specParams)
        : base(p =>
            (string.IsNullOrEmpty(specParams.Search) || p.Name.ToLower().Contains(specParams.Search)) &&
            (!specParams.brandId.HasValue || p.BrandId == specParams.brandId) &&
            (!specParams.categoryId.HasValue || p.CategoryId == specParams.categoryId))
    {
        
    }
    
}