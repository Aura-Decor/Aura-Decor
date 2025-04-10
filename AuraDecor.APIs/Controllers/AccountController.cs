using System.Security.Claims;
using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.APIs.Extensions;
using AuraDecor.APIs.Helpers;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuraDecor.APIs.Controllers;
public class AccountController : ApiBaseController
{
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ITokenService _authService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper,
          IEmailService emailService ,ITokenService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _emailService = emailService;
            _authService = authService;
        }
        [RateLimit(2, 10, RateLimitAlgorithm.SlidingWindow)]  

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)

        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }

            return new UserDto
            {
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager),
                DisplayName = user.DisplayName
            };
        }
        [RateLimit(2, 10, RateLimitAlgorithm.SlidingWindow)]  

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
            {
                return BadRequest(new ApiValidationErrorResponse() { Errors = new[] { "Email address is in use" } });
            }
            if (await _userManager.FindByNameAsync(registerDto.UserName) != null)
            {
                return BadRequest(new ApiValidationErrorResponse() { Errors = new[] { "Username is in use" } });
            }

            var user = new User
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }

            return new UserDto
            {
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager),
                DisplayName = user.DisplayName
            };
        }
        [RateLimit(2, 10, RateLimitAlgorithm.SlidingWindow)]  
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties 
            { 
                RedirectUri = Url.Action("GoogleResponse") 
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
    
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));

            var googleUser = result.Principal;
            var email = googleUser.FindFirst(ClaimTypes.Email)?.Value;
            var name = googleUser.FindFirst(ClaimTypes.Name)?.Value;

            // Check if user exists
            var user = await _userManager.FindByEmailAsync(email);
    
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    UserName = email,
                    DisplayName = name,
                    EmailConfirmed = true 
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return BadRequest(new ApiResponse(400, "Failed to create user"));
            }

            return Ok(new UserDto
            {
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager),
                DisplayName = user.DisplayName
            });
        }

        [HttpGet("twitter-login")]
        public IActionResult TwitterLogin()
        {
            var properties = new AuthenticationProperties 
            { 
                RedirectUri = Url.Action("TwitterResponse") 
            };
            return Challenge(properties, TwitterDefaults.AuthenticationScheme);
        }

        [HttpGet("twitter-response")]
        public async Task<IActionResult> TwitterResponse()
        {
            var result = await HttpContext.AuthenticateAsync(TwitterDefaults.AuthenticationScheme);
    
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));

            var twitterUser = result.Principal;
            var nameIdentifier = twitterUser.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Twitter ID
            var name = twitterUser.FindFirst(ClaimTypes.Name)?.Value;
            var screenName = twitterUser.FindFirst("urn:twitter:screenname")?.Value;
            
            // Twitter doesn't provide email by default, so we'll use the screen name as a unique identifier
            var username = $"twitter_{nameIdentifier}";
            var email = $"{screenName}@twitter.com"; // Create a placeholder email
            
            // Check if user exists by the twitter ID stored in UserName
            var user = await _userManager.FindByNameAsync(username);
    
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    UserName = username,
                    DisplayName = name,
                    EmailConfirmed = true // We consider Twitter accounts as pre-verified
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return BadRequest(new ApiResponse(400, "Failed to create user"));
            }

            return Ok(new UserDto
            {
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager),
                DisplayName = user.DisplayName
            });
        }
        
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindUserWithAddressAsync(User);
            var address = _mapper.Map<Address, AddressDto>(user.Address);
            if (address == null)
            {
                return NotFound(new ApiResponse(404));
                
            }
            return Ok(address);
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            user.Address = _mapper.Map<AddressDto, Address>(addressDto);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded) return Ok(_mapper.Map<Address, AddressDto>(user.Address));
            return BadRequest("Problem updating the user");
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
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        
        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<UserDto>> UpdateUser(UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found"));
            }

            user.DisplayName = updateUserDto.DisplayName;
            user.PhoneNumber = updateUserDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Problem updating the user"));
            }

            return new UserDto
            {
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager),
                DisplayName = user.DisplayName
            };
        }

        [Authorize]
        [HttpPut("updatepassword")]
        public async Task<ActionResult> UpdateUserPassword([FromBody] Dictionary<string, string> passwords)
        {
            if (!passwords.ContainsKey("currentPassword") || !passwords.ContainsKey("newPassword"))
            {
                return BadRequest(new ApiResponse(400, "Current and new passwords are required"));
            }

            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found"));
            }

            var result =
                await _userManager.ChangePasswordAsync(user, passwords["currentPassword"], passwords["newPassword"]);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Problem updating the password"));
            }

            return Ok();
        }
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return Ok();

            var otp = GenerateOtp.GenerateRandomOtp();
            await _emailService.SendOtpEmailAsync(forgotPasswordDto.Email, otp);
    
            return Ok();
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<string>> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            var isValid = await _emailService.ValidateOtpAsync(verifyOtpDto.Email, verifyOtpDto.Otp);
            if (!isValid)
                return BadRequest(new ApiResponse(400, "Invalid or expired OTP"));

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(
                await _userManager.FindByEmailAsync(verifyOtpDto.Email));
    
            return Ok(new { resetToken });
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return BadRequest(new ApiResponse(400, "Invalid request"));

            var result = await _userManager.ResetPasswordAsync(
                user, 
                resetPasswordDto.Token, 
                resetPasswordDto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Could not reset password"));

            return Ok();
        }

           
}
