using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuraDecor.APIs.Controllers;

public class StylesController : ApiBaseController
{
    private readonly IStyleService _styleService;
    private readonly IMapper _mapper;

    public StylesController(IStyleService styleService, IMapper mapper)
    {
        _styleService = styleService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StyleTypeDto>>> GetAllStyles()
    {
        var styles = await _styleService.GetAllStylesAsync();
        var stylesToReturn = _mapper.Map<IReadOnlyList<StyleType>, IReadOnlyList<StyleTypeDto>>(styles);
        return Ok(stylesToReturn);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<StyleTypeDto>> GetStyleById(Guid id)
    {
        var style = await _styleService.GetStyleByIdAsync(id);
        if (style == null)
        {
            return NotFound(new ApiResponse(404, "Style not found"));
        }
        
        return Ok(_mapper.Map<StyleType, StyleTypeDto>(style));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<StyleTypeDto>> CreateStyle(CreateStyleTypeDto createStyleDto)
    {
        var style = _mapper.Map<CreateStyleTypeDto, StyleType>(createStyleDto);
        await _styleService.CreateStyleAsync(style);
        
        return CreatedAtAction(nameof(GetStyleById), new { id = style.Id }, 
            _mapper.Map<StyleType, StyleTypeDto>(style));
    }
}
