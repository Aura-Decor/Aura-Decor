using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }
    
}