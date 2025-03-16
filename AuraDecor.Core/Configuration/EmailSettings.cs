namespace AuraDecor.Core.Configuration;

public record EmailSettings
{
    public string SmtpServer { get; init; }
    public int SmtpPort { get; init; }
    public string SmtpUsername { get; init; }
    public string SmtpPassword { get; init; }
    public string FromName { get; init; }
    public string FromEmail { get; init; }
}