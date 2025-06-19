using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.APIs.Helpers;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications.ProductSpecification;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AuraDecor.Servicies;

namespace AuraDecor.APIs.Controllers
{

    public class FurnitureController : ApiBaseController
    {
        private readonly IFurnitureService _furnitureService;
        private readonly IMapper _mapper;

        public FurnitureController(IFurnitureService furnitureService, IMapper mapper)
        {
            _furnitureService = furnitureService;
            _mapper = mapper;
        }
        [Cached(300)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Furniture>> GetFurnitureById(Guid id)
        {
            var furniture = await _furnitureService.GetFurnitureByIdAsync(id);
            if (furniture == null)
            {
                return NotFound(new ApiResponse(404, "Furniture not found"));
            }
            var furnitureToReturn = _mapper.Map<Furniture, FurnitureToReturnDto>(furniture);
            return Ok(furnitureToReturn);
        }
        [Cached(300)]
        [RateLimit(5, 60, RateLimitAlgorithm.SlidingWindow)]  
        [HttpGet]
        public async Task<ActionResult<Pagination<Furniture>>> GetAllFurniture([FromQuery] FurnitureSpecParams specParams)
        {
            var furnitureList = await _furnitureService.GetAllFurnitureAsync(specParams);
            var count = await _furnitureService.GetCountAsync(specParams);
            var data = _mapper.Map<IReadOnlyList<Furniture>, IReadOnlyList<FurnitureToReturnDto>>(furnitureList);
            return Ok(new Pagination<FurnitureToReturnDto>(specParams.PageIndex, specParams.PageSize, count, data));
        }

        [HttpPost]
        public async Task<ActionResult> AddFurniture([FromForm] AddFurnitureDto furnitureDto)
        {
            var furniture = _mapper.Map<AddFurnitureDto, Furniture>(furnitureDto);
            await _furnitureService.AddFurnitureAsync(furniture, furnitureDto.Image);
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

        [HttpPost("{id}/offers")]
        public async Task<ActionResult> ApplyOffer(Guid id, [FromBody] OfferDto offerDto)
        {
            var furniture = await _furnitureService.GetFurnitureByIdAsync(id);
            if (furniture == null)
            {
                return NotFound(new ApiResponse(404, "Furniture not found"));
            }
    
            await _furnitureService.ApplyOfferAsync(id, offerDto.DiscountPercentage, offerDto.StartDate, offerDto.EndDate);
            return Ok();
        }

        [HttpDelete("{id}/offers")]
        public async Task<ActionResult> RemoveOffer(Guid id)
        {
            var furniture = await _furnitureService.GetFurnitureByIdAsync(id);
            if (furniture == null)
            {
                return NotFound(new ApiResponse(404, "Furniture not found"));
            }
    
            await _furnitureService.RemoveOfferAsync(id);
            return NoContent();
        }
        [Cached(300)]
        [HttpGet("offers/active")]
        public async Task<ActionResult<IReadOnlyList<FurnitureToReturnDto>>> GetFurnituresWithActiveOffers()
        {
            var furnitureWithOffers = await _furnitureService.GetFurnituresWithActiveOffersAsync();
            var data = _mapper.Map<IReadOnlyList<Furniture>, IReadOnlyList<FurnitureToReturnDto>>(furnitureWithOffers);
            return Ok(data);
        }

        [HttpPost("offers/update-status")]
        public async Task<ActionResult> UpdateOffersStatus()
        {
            await _furnitureService.UpdateOffersStatusAsync();
            return Ok();
        }

        [HttpPost("search/text")]
        public async Task<ActionResult<ImageSearchResponseDto>> SearchFurnitureByText([FromForm] string description, [FromForm] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return BadRequest(new ApiResponse(400, "Description is required"));
            }

            var jsonResult = await ((FurnitureService)_furnitureService).SearchFurnitureByTextAsync(description, limit);
            
            try
            {
                var results = JsonSerializer.Deserialize<List<ImageSearchResultDto>>(jsonResult, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                var response = new ImageSearchResponseDto
                {
                    Success = true,
                    Results = results ?? new List<ImageSearchResultDto>()
                };
                
                return Ok(response);
            }
            catch
            {
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResult);
                    return BadRequest(new ApiResponse(400, errorResponse.GetValueOrDefault("Message", "Search failed").ToString()));
                }
                catch
                {
                    return BadRequest(new ApiResponse(400, "Failed to process search results"));
                }
            }
        }

        [HttpPost("search/image")]
        public async Task<ActionResult<ImageSearchResponseDto>> SearchFurnitureByImage([FromForm] ImageSearchDto searchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jsonResult = await ((FurnitureService)_furnitureService).SearchFurnitureByImageAsync(searchDto.File, searchDto.Limit, searchDto.Color);
            
            try
            {
                var results = JsonSerializer.Deserialize<List<ImageSearchResultDto>>(jsonResult, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                var response = new ImageSearchResponseDto
                {
                    Success = true,
                    Results = results ?? new List<ImageSearchResultDto>()
                };
                
                return Ok(response);
            }
            catch
            {
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResult);
                    return BadRequest(new ApiResponse(400, errorResponse.GetValueOrDefault("Message", "Search failed").ToString()));
                }
                catch
                {
                    return BadRequest(new ApiResponse(400, "Failed to process search results"));
                }
            }
        }

    }
}