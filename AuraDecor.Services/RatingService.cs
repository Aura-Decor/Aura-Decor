using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications;

namespace AuraDecor.Services
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Rating> AddRatingAsync(Rating rating)
        {
            var ratingRepo = _unitOfWork.Repository<Rating>();

            var spec = new RatingByUserAndProductSpecification(rating.UserId, rating.ProductId);
            var existingRating = await ratingRepo.GetWithSpecAsync(spec);

            if (existingRating != null)
            {
                existingRating.Stars = rating.Stars;
                existingRating.Review = rating.Review;
                await ratingRepo.UpdateAsync(existingRating);
                await _unitOfWork.CompleteAsync();
                return existingRating;
            }
            
            ratingRepo.Add(rating);
            await _unitOfWork.CompleteAsync();
            return rating;
        }

        public async Task<IReadOnlyList<Rating>> GetRatingsForProductAsync(Guid productId)
        {
            var ratingRepo = _unitOfWork.Repository<Rating>();
            var spec = new RatingsForProductSpecification(productId);
            return await ratingRepo.GetAllWithSpecAsync(spec);
        }
    }
} 