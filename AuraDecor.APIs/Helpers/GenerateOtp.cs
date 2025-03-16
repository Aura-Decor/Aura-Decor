namespace AuraDecor.APIs.Helpers;

public static class GenerateOtp
{
    
    public static string GenerateRandomOtp()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString();
    }
    
}