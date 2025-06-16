using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Services.Contract;

public interface IStyleService
{
    Task<IReadOnlyList<StyleType>> GetAllStylesAsync();
    Task<StyleType> GetStyleByIdAsync(Guid id);
    Task<StyleType> CreateStyleAsync(StyleType style);
}
