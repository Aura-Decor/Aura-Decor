using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications.RefreshTokenSpecifications;

public class RefreshTokenByTokenSpecification : BaseSpecification<RefreshToken>
{
    public RefreshTokenByTokenSpecification(string token)
        : base(rt => rt.Token == token)
    {
    }
}