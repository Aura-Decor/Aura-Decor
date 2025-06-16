using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;

namespace AuraDecor.Services
{
    public class StyleService : IStyleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StyleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<StyleType>> GetAllStylesAsync()
        {
            return await _unitOfWork.Repository<StyleType>().GetAllAsync();
        }

        public async Task<StyleType> GetStyleByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<StyleType>().GetByIdAsync(id);
        }

        public async Task<StyleType> CreateStyleAsync(StyleType style)
        {
            _unitOfWork.Repository<StyleType>().Add(style);
            await _unitOfWork.CompleteAsync();
            return style;
        }
    }
}
