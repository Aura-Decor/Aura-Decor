using AuraDecor.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDecor.Servicies
{
    public interface IFurnitureService
    {
        Task<Furniture> GetFurnitureByIdAsync(Guid id);
        Task<IReadOnlyList<Furniture>> GetAllFurnitureAsync();

        Task AddFurnitureAsync(Furniture furniture);
        Task UpdateFurnitureAsync(Furniture furniture);
        Task DeleteFurnitureAsync(Furniture furniture);


    }
}
