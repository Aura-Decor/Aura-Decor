using System.ComponentModel.DataAnnotations;

namespace AuraDecor.APIs.Dtos.Incoming;

public class ResetPasswordDto
{
    
    [Required]
    [EmailAddress]
    public string Email { get; init; }
    
    [Required]
    public string Token { get; init; }
    
    [Required]
    [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
        ErrorMessage = "Password must have 1 Uppercase, 1 Lowercase, 1 number, 1 non alphanumeric and at least 6 characters")]
    public string NewPassword { get; init; }
}