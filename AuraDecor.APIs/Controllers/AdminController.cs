using System.Security.Claims;
using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuraDecor.APIs.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : ApiBaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _authService;
        private readonly AppDbContext _context;
        private readonly IRoleService _roleService;

        public AdminController(UserManager<User> userManager, ITokenService authService, AppDbContext context, IRoleService roleService)
        {
            _userManager = userManager;
            _authService = authService;
            _context = context;
            _roleService = roleService;
        }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(email);
        return new UserDto
        {
            Email = user.Email,
            Token = await _authService.CreateTokenAsync(user, _userManager),
            DisplayName = user.DisplayName
        };

    }
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            return Ok(users);
        }
        
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var result = await _roleService.CreateRoleAsync(roleName);
            if (!result)
                return BadRequest(new ApiResponse(404,"Role creation failed or role already exists."));

            return Ok("Role created successfully.");
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
        {
            var result = await _roleService.AssignRoleToUserAsync(assignRoleDto.Email, assignRoleDto.RoleName);
            if (!result)
                return BadRequest(new ApiResponse(404,"Role assignment failed or user not found."));

            return Ok("Role assigned successfully.");
        }
    }
    
}