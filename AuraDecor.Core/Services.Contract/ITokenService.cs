using AuraDecor.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuraDecor.Core.Services.Contract;

public interface ITokenService
{
    Task<string> CreateTokenAsync(User user, UserManager<User> userManager);

}