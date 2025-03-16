using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming;

public class VerifyOtpDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }
    
    [Required]
    public string Otp { get; init; }
}