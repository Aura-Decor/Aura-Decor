using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Services.Contract
{
    public interface IFurnitureService
    {
        Task<Furniture> GetFurnitureByIdAsync(Guid id);
        Task<IReadOnlyList<Furniture>> GetAllFurnitureAsync(string sort, Guid? brandId, Guid? categoryId);

        Task AddFurnitureAsync(Furniture furniture);
        Task UpdateFurnitureAsync(Furniture furniture);
        Task DeleteFurnitureAsync(Furniture furniture);
    }
}
