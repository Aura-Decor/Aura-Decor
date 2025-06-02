using System.Text;
using System.Text.Json;
using System.Diagnostics;
using AuraDecor.Core.Configuration;
using AuraDecor.Core.Messages;
using AuraDecor.Core.Services.Contract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using StackExchange.Redis;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AuraDecor.Services;

public class EmailService : IEmailService, IDisposable
{
    private readonly EmailSettings _emailSettings;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IDatabase _redisDb;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogger<EmailService> _logger;
    private const int OTP_EXPIRY = 5;

    private static IConnection _connection;
    private static IModel _channel;
    private readonly object _connectionLock = new object();

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        IOptions<RabbitMqSettings> rabbitMqSettings,
        IConnectionMultiplexer redis,
        IEmailTemplateService emailTemplateService,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
        _rabbitMqSettings = rabbitMqSettings.Value ?? throw new ArgumentNullException(nameof(rabbitMqSettings));
        _redisDb = redis.GetDatabase();
        _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InitializeRabbitMqConnection();
    }

    private void InitializeRabbitMqConnection()
    {
        if (_connection != null && _connection.IsOpen)
            return;

        lock (_connectionLock)
        {
            if (_connection != null && _connection.IsOpen)
                return;

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_rabbitMqSettings.GetConnectionString()),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_rabbitMqSettings.Exchanges.EmailExchange, ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(_rabbitMqSettings.Queues.OtpEmails, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(_rabbitMqSettings.Queues.NotificationEmails, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(_rabbitMqSettings.Queues.OtpEmails, _rabbitMqSettings.Exchanges.EmailExchange, _rabbitMqSettings.Queues.OtpEmails);
            _channel.QueueBind(_rabbitMqSettings.Queues.NotificationEmails, _rabbitMqSettings.Exchanges.EmailExchange, _rabbitMqSettings.Queues.NotificationEmails);
        }
    }

    public async Task SendOtpEmailAsync(string email, string otp)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var otpEmailMessage = new OtpEmailMessage
            {
                ToEmail = email,
                Subject = "Your AuraDecor Verification Code",
                HtmlBody = _emailTemplateService.CreateOtpEmailTemplate(otp, OTP_EXPIRY),
                FromEmail = _emailSettings.FromEmail,
                FromName = _emailSettings.FromName,
                Otp = otp,
                ExpiryMinutes = OTP_EXPIRY
            };

            PublishToQueue(
                _rabbitMqSettings.Queues.OtpEmails,
                _rabbitMqSettings.Exchanges.EmailExchange,
                otpEmailMessage
            );

            await _redisDb.StringSetAsync(
                $"otp:{email}",
                otp,
                TimeSpan.FromMinutes(OTP_EXPIRY)
            );

            _logger.LogInformation("OTP email for {Email} queued successfully in {ElapsedMs}ms", email, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue OTP email for {Email} after {ElapsedMs}ms", email, stopwatch.ElapsedMilliseconds);
            await SendDirectEmailAsync(email, "Your AuraDecor Verification Code", _emailTemplateService.CreateOtpEmailTemplate(otp, OTP_EXPIRY));
        }
    }

    public async Task<bool> ValidateOtpAsync(string email, string otp)
    {
        var storedOtp = await _redisDb.StringGetAsync($"otp:{email}");
        if (!storedOtp.IsNull && storedOtp.ToString() == otp)
        {
            await _redisDb.KeyDeleteAsync($"otp:{email}"); 
            return true;
        }
        return false;
    }

    public async Task SendNotificationEmailAsync(string email, string subject, string htmlBody)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var notificationEmail = new EmailMessage
            {
                ToEmail = email,
                Subject = subject,
                HtmlBody = htmlBody,
                FromEmail = _emailSettings.FromEmail,
                FromName = _emailSettings.FromName
            };

            PublishToQueue(
                _rabbitMqSettings.Queues.NotificationEmails,
                _rabbitMqSettings.Exchanges.EmailExchange,
                notificationEmail
            );

            _logger.LogInformation("Notification email for {Email} queued successfully in {ElapsedMs}ms", email, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue notification email for {Email} after {ElapsedMs}ms", email, stopwatch.ElapsedMilliseconds);
            await SendDirectEmailAsync(email, subject, htmlBody);
        }
    }

    private void PublishToQueue<T>(string queueName, string exchangeName, T message)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(message);
            var messageBody = Encoding.UTF8.GetBytes(messageJson);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.CorrelationId = Guid.NewGuid().ToString();

            _channel.BasicPublish(
                exchange: exchangeName,
                routingKey: queueName,
                basicProperties: properties,
                body: messageBody
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing to queue {QueueName}", queueName);
            throw;
        }
    }

    private async Task SendDirectEmailAsync(string email, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            await client.SendAsync(message);
            _logger.LogInformation("Email sent directly to {Email} as fallback", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email directly to {Email}", email);
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _connection?.Close();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during disposal of RabbitMQ resources");
        }
        finally
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}