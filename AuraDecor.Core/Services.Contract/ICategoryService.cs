using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Services.Contract;

public interface ICategoryService
{
    Task<IReadOnlyList<FurnitureCategory>> GetAllCategoriesAsync();
    Task<FurnitureCategory> GetCategoryByIdAsync(Guid id);
    Task<FurnitureCategory> CreateCategoryAsync(FurnitureCategory category);
}
