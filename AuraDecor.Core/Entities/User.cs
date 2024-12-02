using Microsoft.AspNetCore.Identity;

namespace AuraDecor.Core.Entities;

public class User : IdentityUser
{
    public string DisplayName { get; set; }
}