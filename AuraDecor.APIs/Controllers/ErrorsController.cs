using AuraDecor.APIs.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuraDecor.APIs.Controllers;
[Route("errors/{code}")]
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class ErrorsController : ControllerBase
{
    [HttpGet]
    public IActionResult Error(int code)
    {
        return NotFound(new ApiResponse(code));
    }
    
}