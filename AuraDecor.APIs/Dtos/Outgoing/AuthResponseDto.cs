namespace AuraDecor.APIs.Dtos.Outgoing;

public class AuthResponseDto
{
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}