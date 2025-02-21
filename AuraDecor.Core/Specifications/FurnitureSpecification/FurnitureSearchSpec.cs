using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.ProductSpecification;

public class FurnitureSearchSpec : BaseSpecification<Furniture>
{
    public FurnitureSearchSpec(string searchTerm)
        : base(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
    {
        AddIncludes();
    }

    private void AddIncludes()
    {
        Includes.Add(p => p.Brand);
        Includes.Add(p => p.Category);
    }
}