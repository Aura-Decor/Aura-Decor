using AuraDecor.Core.Entities;
using AuraDecor.Core.Specifications.ProductSpecification;

namespace AuraDecor.Core.Services.Contract
{
    public interface IFurnitureService
    {
        Task<Furniture> GetFurnitureByIdAsync(Guid id);
        Task<IReadOnlyList<Furniture>> GetAllFurnitureAsync(FurnitureSpecParams specParams);
        Task<IReadOnlyList<Furniture>> SearchFurnitureAsync(string searchTerm);

        Task AddFurnitureAsync(Furniture furniture);
        Task UpdateFurnitureAsync(Furniture furniture);
        Task DeleteFurnitureAsync(Furniture furniture);
        
        Task<int> GetCountAsync(FurnitureSpecParams specParams);

    }
}
