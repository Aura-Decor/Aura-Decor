using System.Collections.Generic;
using System.Threading.Tasks;
using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Services.Contract
{
    public interface IRatingService
    {
        Task<Rating> AddRatingAsync(Rating rating);
        Task<IReadOnlyList<Rating>> GetRatingsForProductAsync(Guid productId);
    }
} 