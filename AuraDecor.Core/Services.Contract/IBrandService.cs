using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Services.Contract;

public interface IBrandService
{
    Task<IReadOnlyList<FurnitureBrand>> GetAllBrandsAsync();
    Task<FurnitureBrand> GetBrandByIdAsync(Guid id);
    Task<FurnitureBrand> CreateBrandAsync(FurnitureBrand brand);
}
