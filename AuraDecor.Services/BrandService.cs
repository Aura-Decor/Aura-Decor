using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;

namespace AuraDecor.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<FurnitureBrand>> GetAllBrandsAsync()
        {
            return await _unitOfWork.Repository<FurnitureBrand>().GetAllAsync();
        }

        public async Task<FurnitureBrand> GetBrandByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<FurnitureBrand>().GetByIdAsync(id);
        }

        public async Task<FurnitureBrand> CreateBrandAsync(FurnitureBrand brand)
        {
            _unitOfWork.Repository<FurnitureBrand>().Add(brand);
            await _unitOfWork.CompleteAsync();
            return brand;
        }
    }
}
