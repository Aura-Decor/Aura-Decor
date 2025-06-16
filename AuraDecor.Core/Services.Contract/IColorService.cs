using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Services.Contract;

public interface IColorService
{
    Task<IReadOnlyList<Color>> GetAllColorsAsync();
    Task<Color> GetColorByIdAsync(Guid id);
    Task<Color> CreateColorAsync(Color color);
}
