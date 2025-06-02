using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Services.Contract;
using AuraDecor.Core.Specifications;
using AuraDecor.Core.Specifications.RefreshTokenSpecifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuraDecor.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;

    public TokenService(IConfiguration config, IUnitOfWork unitOfWork)
    {
        _config = config;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<string> CreateTokenAsync(User user, UserManager<User> userManager)
    {
        var authClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var userRoles = await userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]));
        var token = new JwtSecurityToken(
            issuer: _config["JWT:ValidIssuer"],
            audience: _config["JWT:ValidAudience"],
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JWT:AccessTokenExpiryMinutes"] ?? "30")),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<RefreshToken> GenerateRefreshTokenAsync(string userId, string jwtId)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["JWT:RefreshTokenExpiryDays"] ?? "7")),
            Created = DateTime.UtcNow,
            UserId = userId,
            JwtId = jwtId,
            IsRevoked = false
        };
        
        return refreshToken;
    }
    
    public async Task<bool> StoreRefreshTokenAsync(RefreshToken refreshToken)
    {
        try
        {
            _unitOfWork.Repository<RefreshToken>().Add(refreshToken);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<(string AccessToken, RefreshToken RefreshToken)> RefreshTokenAsync(
        string accessToken, 
        string refreshToken, 
        UserManager<User> userManager)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            throw new SecurityTokenException("Invalid access token");
        }
        
        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.NameId);
        var jwtId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
        
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new SecurityTokenException("User not found");
        }
        
        var spec = new RefreshTokenSpecification(refreshToken, jwtId, userId);
        var storedToken = await _unitOfWork.Repository<RefreshToken>().GetWithSpecAsync(spec);
        
        if (storedToken == null)
        {
            throw new SecurityTokenException("Refresh token not found");
        }
        
        if (storedToken.IsRevoked)
        {
            await RevokeDescendantRefreshTokensAsync(storedToken, "Attempted reuse of revoked ancestor token");
            await _unitOfWork.CompleteAsync();
            
            throw new SecurityTokenException("Refresh token has been revoked");
        }
        
        if (!storedToken.IsActive)
        {
            throw new SecurityTokenException("Refresh token has expired");
        }
        
        var newAccessToken = await CreateTokenAsync(user, userManager);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(newAccessToken);
        var newJwtId = jwtToken.Id;
        
        storedToken.ReplacedByToken = refreshToken;
        storedToken.IsRevoked = true;
        await _unitOfWork.Repository<RefreshToken>().UpdateAsync(storedToken);
        
        var newRefreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["JWT:RefreshTokenExpiryDays"] ?? "7")),
            Created = DateTime.UtcNow,
            UserId = userId,
            JwtId = newJwtId,
            IsRevoked = false
        };
        
        _unitOfWork.Repository<RefreshToken>().Add(newRefreshToken);
        await _unitOfWork.CompleteAsync();
        
        return (newAccessToken, newRefreshToken);
    }
    
    public async Task<bool> RevokeTokenAsync(string userId, string refreshToken)
    {
        var spec = new RefreshTokenByTokenAndUserSpecification(refreshToken, userId);
        var storedToken = await _unitOfWork.Repository<RefreshToken>().GetWithSpecAsync(spec);
        
        if (storedToken == null)
        {
            return false;
        }
        
        if (storedToken.IsRevoked)
        {
            return true; 
        }
        
        storedToken.IsRevoked = true;
        await _unitOfWork.Repository<RefreshToken>().UpdateAsync(storedToken);
        await RevokeDescendantRefreshTokensAsync(storedToken, "Explicit revocation");
        await _unitOfWork.CompleteAsync();
        
        return true;
    }
    
    public async Task<bool> ValidateAccessTokenAsync(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]);
        
        try
        {
            tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config["JWT:ValidIssuer"],
                ValidateAudience = true,
                ValidAudience = _config["JWT:ValidAudience"],
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecretKey"])),
            ValidateIssuer = true,
            ValidIssuer = _config["JWT:ValidIssuer"],
            ValidateAudience = true,
            ValidAudience = _config["JWT:ValidAudience"],
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
    
    private async Task RevokeDescendantRefreshTokensAsync(RefreshToken refreshToken, string reason)
    {
        if (string.IsNullOrEmpty(refreshToken.ReplacedByToken)) return;
        
        var spec = new RefreshTokenByTokenSpecification(refreshToken.ReplacedByToken);
        var childToken = await _unitOfWork.Repository<RefreshToken>().GetWithSpecAsync(spec);
        
        if (childToken == null) return;
        
        if (childToken.IsActive)
        {
            childToken.IsRevoked = true;
            await _unitOfWork.Repository<RefreshToken>().UpdateAsync(childToken);
        }
        else
        {
            return;
        }
        
        await RevokeDescendantRefreshTokensAsync(childToken, reason);
    }
}
