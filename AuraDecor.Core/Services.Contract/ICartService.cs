using AuraDecor.Core.Entities;
namespace AuraDecor.Core.Services.Contract
{
    public interface ICartService
    {


        Task<Cart> GetCartByUserIdAsync(string Id);
        Task AddToCartAsync(string userId, Guid furnitureId, int quantity);
        Task RemoveFromCartAsync(string userId, Guid furnitureId);
    }
}
