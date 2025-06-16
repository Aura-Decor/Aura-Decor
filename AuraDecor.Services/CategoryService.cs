using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;

namespace AuraDecor.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<FurnitureCategory>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Repository<FurnitureCategory>().GetAllAsync();
        }

        public async Task<FurnitureCategory> GetCategoryByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<FurnitureCategory>().GetByIdAsync(id);
        }

        public async Task<FurnitureCategory> CreateCategoryAsync(FurnitureCategory category)
        {
            _unitOfWork.Repository<FurnitureCategory>().Add(category);
            await _unitOfWork.CompleteAsync();
            return category;
        }
    }
}
