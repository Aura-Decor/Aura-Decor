using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.RefreshTokenSpecifications;

public class RefreshTokenByTokenAndUserSpecification : BaseSpecification<RefreshToken>
{
    public RefreshTokenByTokenAndUserSpecification(string token, string userId)
        : base(rt => rt.Token == token && rt.UserId == userId)
    {
    }
}