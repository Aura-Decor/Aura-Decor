using AuraDecor.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuraDecor.Core.Services.Contract;

public interface ITokenService
{
    Task<string> CreateTokenAsync(User user, UserManager<User> userManager);
    
    Task<RefreshToken> GenerateRefreshTokenAsync(string userId, string jwtId);
    
    Task<bool> StoreRefreshTokenAsync(RefreshToken refreshToken);
    
    Task<(string AccessToken, RefreshToken RefreshToken)> RefreshTokenAsync(
        string accessToken, 
        string refreshToken, 
        UserManager<User> userManager);
        
    Task<bool> RevokeTokenAsync(string userId, string refreshToken);
    
    Task<bool> ValidateAccessTokenAsync(string accessToken);
}