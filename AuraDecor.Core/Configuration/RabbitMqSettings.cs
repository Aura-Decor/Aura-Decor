namespace AuraDecor.Core.Configuration;

public class RabbitMqSettings
{
    public string ConnectionString { get; set; }
    public string Uri { get; set; } 
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; } = "/";
    public int Port { get; set; } = 5672;
    public int TlsPort { get; set; } = 5671;
    public bool UseTls { get; set; }

    public QueueSettings Queues { get; set; } = new QueueSettings();
    public ExchangeSettings Exchanges { get; set; } = new ExchangeSettings();
    public RetryPolicySettings RetryPolicy { get; set; } = new RetryPolicySettings();

    public string GetConnectionString() => !string.IsNullOrEmpty(Uri) ? Uri : ConnectionString;

    public class QueueSettings
    {
        public string EmailNotifications { get; set; } = "email-notifications";
        public string OtpEmails { get; set; } = "otp-emails";
        public string NotificationEmails { get; set; } = "notification-emails";
    }

    public class ExchangeSettings
    {
        public string EmailExchange { get; set; } = "email-exchange";
    }

    public class RetryPolicySettings
    {
        public int MaxRetries { get; set; } = 3;
        public int RetryDelayMs { get; set; } = 5000;
    }
}