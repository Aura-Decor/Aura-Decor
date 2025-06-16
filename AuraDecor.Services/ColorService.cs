using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;

namespace AuraDecor.Services
{
    public class ColorService : IColorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ColorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Color>> GetAllColorsAsync()
        {
            return await _unitOfWork.Repository<Color>().GetAllAsync();
        }

        public async Task<Color> GetColorByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<Color>().GetByIdAsync(id);
        }

        public async Task<Color> CreateColorAsync(Color color)
        {
            _unitOfWork.Repository<Color>().Add(color);
            await _unitOfWork.CompleteAsync();
            return color;
        }
    }
}
