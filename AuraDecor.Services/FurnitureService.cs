using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Repositoriy.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDecor.Core.Specifications.ProductSpecification;
using Microsoft.AspNetCore.Http;

namespace AuraDecor.Servicies
{
    public class FurnitureService : IFurnitureService
    {
        private readonly IUnitOfWork _unitOfWork;


        public FurnitureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddFurnitureAsync(Furniture furniture, IFormFile file)
        {
            string furniturePictureUrl = await FileUploadService.UploadAsync(file);
            furniture.PictureUrl = furniturePictureUrl;

            _unitOfWork.Repository<Furniture>().Add(furniture);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteFurnitureAsync(Furniture furniture)
        {
            _unitOfWork.Repository<Furniture>().DeleteAsync(furniture);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IReadOnlyList<Furniture>> GetAllFurnitureAsync(FurnitureSpecParams specParams)
        {
            var spec = new FurnitureWithCategoryAndBrandSpec(specParams);
            return await _unitOfWork.Repository<Furniture>().GetAllWithSpecAsync(spec);
        }

        public async Task<Furniture> GetFurnitureByIdAsync(Guid id)
        {
            var spec = new FurnitureWithCategoryAndBrandSpec(id);
            return await _unitOfWork.Repository<Furniture>().GetWithSpecAsync(spec);
        }

        public async Task UpdateFurnitureAsync(Furniture furniture)
        {
            _unitOfWork.Repository<Furniture>().UpdateAsync(furniture);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<int> GetCountAsync(FurnitureSpecParams specParams)
        {
            var countSpec = new FurnitureWithFiltrationForCountSpec(specParams);
            var count = await _unitOfWork.Repository<Furniture>().GetCountAsync(countSpec);
            return count;
        }

        public async Task ApplyOfferAsync(Guid furnitureId, decimal discountPercentage, DateTime startDate,
            DateTime endDate)
        {
            var furniture = await _unitOfWork.Repository<Furniture>().GetByIdAsync(furnitureId);
            if (furniture == null)
                return;

            furniture.HasOffer = true;
            furniture.DiscountPercentage = discountPercentage;
            furniture.OfferStartDate = startDate;
            furniture.OfferEndDate = endDate;

            // Calculate discounted price
            decimal discount = furniture.Price * (discountPercentage / 100);
            furniture.DiscountedPrice = furniture.Price - discount;

            _unitOfWork.Repository<Furniture>().UpdateAsync(furniture);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveOfferAsync(Guid furnitureId)
        {
            var furniture = await _unitOfWork.Repository<Furniture>().GetByIdAsync(furnitureId);
            if (furniture == null)
                return;

            furniture.HasOffer = false;
            furniture.DiscountPercentage = null;
            furniture.DiscountedPrice = null;
            furniture.OfferStartDate = null;
            furniture.OfferEndDate = null;

            _unitOfWork.Repository<Furniture>().UpdateAsync(furniture);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IReadOnlyList<Furniture>> GetFurnituresWithActiveOffersAsync()
        {
            var spec = new FurnitureWithActiveOffersSpec();
            return await _unitOfWork.Repository<Furniture>().GetAllWithSpecAsync(spec);
        }

        public async Task UpdateOffersStatusAsync()
        {
            // Get all furniture with offers
            var spec = new FurnitureWithOffersSpec();
            var furnituresWithOffers = await _unitOfWork.Repository<Furniture>().GetAllWithSpecAsync(spec);

            var currentDate = DateTime.UtcNow;
            var updatedAny = false;

            foreach (var furniture in furnituresWithOffers)
            {
                // Check if offer has expired
                if (furniture.HasOffer && furniture.OfferEndDate.HasValue && furniture.OfferEndDate.Value < currentDate)
                {
                    furniture.HasOffer = false;
                    furniture.DiscountPercentage = null;
                    furniture.DiscountedPrice = null;
                    furniture.OfferStartDate = null;
                    furniture.OfferEndDate = null;

                    _unitOfWork.Repository<Furniture>().UpdateAsync(furniture);
                    updatedAny = true;
                }
                // Check if offer should be activated
                else if (!furniture.HasOffer && furniture.OfferStartDate.HasValue &&
                         furniture.OfferStartDate.Value <= currentDate &&
                         furniture.OfferEndDate.HasValue && furniture.OfferEndDate.Value > currentDate)
                {
                    furniture.HasOffer = true;

                    // Recalculate discounted price in case price was updated
                    if (furniture.DiscountPercentage.HasValue)
                    {
                        decimal discount = furniture.Price * (furniture.DiscountPercentage.Value / 100);
                        furniture.DiscountedPrice = furniture.Price - discount;
                    }

                    _unitOfWork.Repository<Furniture>().UpdateAsync(furniture);
                    updatedAny = true;
                }
            }

            if (updatedAny)
            {
                await _unitOfWork.CompleteAsync();
            }
        }


    }
}
