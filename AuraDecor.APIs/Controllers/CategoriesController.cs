using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuraDecor.APIs.Controllers;

public class CategoriesController : ApiBaseController
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public CategoriesController(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        var categoriesToReturn = _mapper.Map<IReadOnlyList<FurnitureCategory>, IReadOnlyList<CategoryDto>>(categories);
        return Ok(categoriesToReturn);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(Guid id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound(new ApiResponse(404, "Category not found"));
        }
        
        return Ok(_mapper.Map<FurnitureCategory, CategoryDto>(category));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        var category = _mapper.Map<CreateCategoryDto, FurnitureCategory>(createCategoryDto);
        await _categoryService.CreateCategoryAsync(category);
        
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, 
            _mapper.Map<FurnitureCategory, CategoryDto>(category));
    }
}
