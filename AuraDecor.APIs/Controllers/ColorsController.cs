using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuraDecor.APIs.Controllers;

public class ColorsController : ApiBaseController
{
    private readonly IColorService _colorService;
    private readonly IMapper _mapper;

    public ColorsController(IColorService colorService, IMapper mapper)
    {
        _colorService = colorService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ColorDto>>> GetAllColors()
    {
        var colors = await _colorService.GetAllColorsAsync();
        var colorsToReturn = _mapper.Map<IReadOnlyList<Color>, IReadOnlyList<ColorDto>>(colors);
        return Ok(colorsToReturn);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ColorDto>> GetColorById(Guid id)
    {
        var color = await _colorService.GetColorByIdAsync(id);
        if (color == null)
        {
            return NotFound(new ApiResponse(404, "Color not found"));
        }
        
        return Ok(_mapper.Map<Color, ColorDto>(color));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ColorDto>> CreateColor(CreateColorDto createColorDto)
    {
        var color = _mapper.Map<CreateColorDto, Color>(createColorDto);
        await _colorService.CreateColorAsync(color);
        
        return CreatedAtAction(nameof(GetColorById), new { id = color.Id }, 
            _mapper.Map<Color, ColorDto>(color));
    }
}
