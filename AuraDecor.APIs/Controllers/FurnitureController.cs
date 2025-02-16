using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using Microsoft.AspNetCore.Mvc;

namespace AuraDecor.APIs.Controllers
{
    
    public class FurnitureController : ApiBaseController
    {
        private readonly IFurnitureService _furnitureService;

        public FurnitureController(IFurnitureService furnitureService)
        {
            _furnitureService = furnitureService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Furniture>> GetFurnitureById(Guid id)
        {
            var furniture = await _furnitureService.GetFurnitureByIdAsync(id);
            if (furniture == null)
            {
                return NotFound(new ApiResponse(404, "Furniture not found"));
            }
            return Ok(furniture);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Furniture>>> GetAllFurniture()
        {
            var furnitureList = await _furnitureService.GetAllFurnitureAsync();
            return Ok(furnitureList);
        }

        [HttpPost]
        public async Task<ActionResult> AddFurniture(Furniture furniture)
        {
            await _furnitureService.AddFurnitureAsync(furniture);
            return CreatedAtAction(nameof(GetFurnitureById), new { id = furniture.Id }, furniture);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFurniture(Guid id, Furniture furniture)
        {
            if (id != furniture.Id)
            {
                return BadRequest(new ApiResponse(400, "ID mismatch"));
            }

            await _furnitureService.UpdateFurnitureAsync(furniture);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFurniture(Guid id)
        {
            var furniture = await _furnitureService.GetFurnitureByIdAsync(id);
            if (furniture == null)
            {
                return NotFound(new ApiResponse(404, "Furniture not found"));
            }

            await _furnitureService.DeleteFurnitureAsync(furniture);
            return NoContent();
        }
    }
}