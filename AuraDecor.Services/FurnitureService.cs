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
            var spec = new FurnitureWithDetailsSpec(specParams);
            return await _unitOfWork.Repository<Furniture>().GetAllWithSpecAsync(spec);
        }

        public async Task<Furniture> GetFurnitureByIdAsync(Guid id)
        {
            var spec = new FurnitureWithDetailsSpec(id);
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

        public async Task<ImageSearchResponseDto> SearchFurnitureByTextAsync(string description, int limit = 10)
        {
            try
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(description), "description");
                formData.Add(new StringContent(limit.ToString()), "limit");

                var response = await _httpClient.PostAsync(
                    "https://A7medAyman-image-text-search.hf.space/api/v1/text/TextSearchQuery", 
                    formData);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var results = JsonSerializer.Deserialize<List<ImageSearchResultDto>>(jsonResponse, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return new ImageSearchResponseDto
                    {
                        Success = true,
                        Results = results ?? new List<ImageSearchResultDto>()
                    };
                }
                else
                {
                    return new ImageSearchResponseDto
                    {
                        Success = false,
                        Message = $"API request failed with status: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ImageSearchResponseDto
                {
                    Success = false,
                    Message = $"Error searching by text: {ex.Message}"
                };
            }
        }

        public async Task<ImageSearchResponseDto> SearchFurnitureByImageAsync(IFormFile image, int limit = 10, string? color = null)
        {
            try
            {
                var formData = new MultipartFormDataContent();
                
                // Add the image file
                using var imageStream = image.OpenReadStream();
                var imageContent = new StreamContent(imageStream);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                formData.Add(imageContent, "file", image.FileName);
                
                // Add optional parameters
                formData.Add(new StringContent(limit.ToString()), "limit");
                if (!string.IsNullOrEmpty(color))
                {
                    formData.Add(new StringContent(color), "color");
                }

                var response = await _httpClient.PostAsync(
                    "https://A7medAyman-image-text-search.hf.space/api/v1/image/ImageSearchQuery", 
                    formData);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var results = JsonSerializer.Deserialize<List<ImageSearchResultDto>>(jsonResponse, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return new ImageSearchResponseDto
                    {
                        Success = true,
                        Results = results ?? new List<ImageSearchResultDto>()
                    };
                }
                else
                {
                    return new ImageSearchResponseDto
                    {
                        Success = false,
                        Message = $"API request failed with status: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ImageSearchResponseDto
                {
                    Success = false,
                    Message = $"Error searching by image: {ex.Message}"
                };
            }
        }


    }
}
