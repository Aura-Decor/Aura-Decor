using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.RefreshTokenSpecifications;

public class RefreshTokenSpecification : BaseSpecification<RefreshToken>
{
    public RefreshTokenSpecification(string token, string jwtId, string userId)
        : base(rt => rt.Token == token && rt.JwtId == jwtId && rt.UserId == userId)
    {
    }
}