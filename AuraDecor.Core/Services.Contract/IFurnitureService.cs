using AuraDecor.Core.Entities;
using AuraDecor.Core.Specifications.ProductSpecification;
using Microsoft.AspNetCore.Http;

namespace AuraDecor.Core.Services.Contract
{
    public interface IFurnitureService
    {
        Task<Furniture> GetFurnitureByIdAsync(Guid id);
        Task<IReadOnlyList<Furniture>> GetAllFurnitureAsync(FurnitureSpecParams specParams);

        Task AddFurnitureAsync(Furniture furniture, IFormFile file);
        Task UpdateFurnitureAsync(Furniture furniture);
        Task UpdateFurniturePartialAsync(Guid furnitureId, string? name = null, string? description = null, 
            string? furnitureModel = null, decimal? price = null, Guid? brandId = null, Guid? categoryId = null, 
            Guid? styleTypeId = null, Guid? colorId = null, IFormFile? image = null);
        Task DeleteFurnitureAsync(Furniture furniture);
        
        Task ApplyOfferAsync(Guid furnitureId, decimal discountPercentage, DateTime startDate, DateTime endDate);
        Task RemoveOfferAsync(Guid furnitureId);
        Task<IReadOnlyList<Furniture>> GetFurnituresWithActiveOffersAsync();
        Task UpdateOffersStatusAsync(); 
        Task<int> GetCountAsync(FurnitureSpecParams specParams);
    }


    
}