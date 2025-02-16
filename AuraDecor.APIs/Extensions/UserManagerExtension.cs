using System.Security.Claims;
using AuraDecor.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuraDecor.APIs.Extensions;

public static class UserManagerExtension
{
    public static async Task<User> FindUserWithAddressAsync(this UserManager<User> userManager, ClaimsPrincipal User)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        return await userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(x => x.Email == email);
    }
}