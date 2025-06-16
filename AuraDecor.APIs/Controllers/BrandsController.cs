using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuraDecor.APIs.Controllers;

public class BrandsController : ApiBaseController
{
    private readonly IBrandService _brandService;
    private readonly IMapper _mapper;

    public BrandsController(IBrandService brandService, IMapper mapper)
    {
        _brandService = brandService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetAllBrands()
    {
        var brands = await _brandService.GetAllBrandsAsync();
        var brandsToReturn = _mapper.Map<IReadOnlyList<FurnitureBrand>, IReadOnlyList<BrandDto>>(brands);
        return Ok(brandsToReturn);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<BrandDto>> GetBrandById(Guid id)
    {
        var brand = await _brandService.GetBrandByIdAsync(id);
        if (brand == null)
        {
            return NotFound(new ApiResponse(404, "Brand not found"));
        }
        
        return Ok(_mapper.Map<FurnitureBrand, BrandDto>(brand));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<BrandDto>> CreateBrand(CreateBrandDto createBrandDto)
    {
        var brand = _mapper.Map<CreateBrandDto, FurnitureBrand>(createBrandDto);
        await _brandService.CreateBrandAsync(brand);
        
        return CreatedAtAction(nameof(GetBrandById), new { id = brand.Id }, _mapper.Map<FurnitureBrand, BrandDto>(brand));
    }
}
