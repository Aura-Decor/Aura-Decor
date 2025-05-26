using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using AuraDecor.Core.Configuration;
using AuraDecor.Core.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AuraDecor.Servicies;

public class EmailQueueConsumer : BackgroundService
{
    private readonly EmailSettings _emailSettings;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly ILogger<EmailQueueConsumer> _logger;
    private IConnection _connection;
    private RabbitMQ.Client.IModel _channel;
    private int _retryCount = 0;

    public EmailQueueConsumer(
        IOptions<EmailSettings> emailSettings,
        IOptions<RabbitMqSettings> rabbitMqSettings,
        ILogger<EmailQueueConsumer> logger)
    {
        _emailSettings = emailSettings.Value;
        _rabbitMqSettings = rabbitMqSettings.Value;
        _logger = logger;
        InitializeRabbitMqConnection();
    }

    private void InitializeRabbitMqConnection()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_rabbitMqSettings.Uri),
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                _rabbitMqSettings.Exchanges.EmailExchange,
                ExchangeType.Direct,
                durable: true
            );

            // OTP Emails Queue
            _channel.QueueDeclare(
                _rabbitMqSettings.Queues.OtpEmails,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            _channel.QueueBind(
                _rabbitMqSettings.Queues.OtpEmails,
                _rabbitMqSettings.Exchanges.EmailExchange,
                _rabbitMqSettings.Queues.OtpEmails
            );

            // Notification Emails Queue
            _channel.QueueDeclare(
                _rabbitMqSettings.Queues.NotificationEmails,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            _channel.QueueBind(
                _rabbitMqSettings.Queues.NotificationEmails,
                _rabbitMqSettings.Exchanges.EmailExchange,
                _rabbitMqSettings.Queues.NotificationEmails
            );

            // General Email Notifications Queue
            _channel.QueueDeclare(
                _rabbitMqSettings.Queues.EmailNotifications,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            _channel.QueueBind(
                _rabbitMqSettings.Queues.EmailNotifications,
                _rabbitMqSettings.Exchanges.EmailExchange,
                _rabbitMqSettings.Queues.EmailNotifications
            );

            _channel.BasicQos(0, 5, false);
            
            _logger.LogInformation("Successfully connected to RabbitMQ in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            _retryCount = 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error connecting to RabbitMQ after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            
            if (_retryCount < _rabbitMqSettings.RetryPolicy.MaxRetries)
            {
                var delay = _rabbitMqSettings.RetryPolicy.RetryDelayMs * Math.Pow(2, _retryCount);
                _logger.LogInformation("Retrying connection in {Delay}ms", delay);
                
                _retryCount++;
                Thread.Sleep(TimeSpan.FromMilliseconds(delay));
                InitializeRabbitMqConnection();
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await StartConsumingQueue(_rabbitMqSettings.Queues.OtpEmails, ProcessOtpEmail, stoppingToken);
            await StartConsumingQueue(_rabbitMqSettings.Queues.NotificationEmails, ProcessNotificationEmail, stoppingToken);
            await StartConsumingQueue(_rabbitMqSettings.Queues.EmailNotifications, ProcessGeneralEmail, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_connection == null || !_connection.IsOpen)
                {
                    _logger.LogWarning("RabbitMQ connection lost. Attempting to reconnect...");
                    InitializeRabbitMqConnection();
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Email queue consumer is shutting down");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in email queue consumer");
        }
        finally
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }

    private async Task StartConsumingQueue(
        string queueName, 
        Func<string, CancellationToken, Task> messageProcessor,
        CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (sender, eventArgs) =>
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                await messageProcessor(message, cancellationToken);
                
                _channel.BasicAck(eventArgs.DeliveryTag, false);
                _logger.LogInformation("Message from {QueueName} processed in {ElapsedMs}ms", queueName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from queue {QueueName} after {ElapsedMs}ms", queueName, stopwatch.ElapsedMilliseconds);
                _channel.BasicNack(eventArgs.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queueName, false, consumer);
        _logger.LogInformation("Started consuming messages from queue {QueueName}", queueName);
        
        await Task.CompletedTask;
    }

    private async Task ProcessOtpEmail(string messageJson, CancellationToken cancellationToken)
    {
        var message = JsonSerializer.Deserialize<OtpEmailMessage>(messageJson);
        
        if (message == null)
        {
            _logger.LogWarning("Received null OTP email message");
            return;
        }
        
        _logger.LogInformation("Processing OTP email for {Email}", message.ToEmail);
        await SendEmailAsync(
            message.ToEmail,
            message.Subject,
            message.HtmlBody,
            cancellationToken
        );
    }

    private async Task ProcessNotificationEmail(string messageJson, CancellationToken cancellationToken)
    {
        var message = JsonSerializer.Deserialize<NotificationEmailMessage>(messageJson);
        
        if (message == null)
        {
            _logger.LogWarning("Received null notification email message");
            return;
        }
        
        _logger.LogInformation("Processing notification email for {Email}", message.ToEmail);
        await SendEmailAsync(
            message.ToEmail,
            message.Subject,
            message.HtmlBody,
            cancellationToken
        );
    }

    private async Task ProcessGeneralEmail(string messageJson, CancellationToken cancellationToken)
    {
        var message = JsonSerializer.Deserialize<EmailMessage>(messageJson);
        
        if (message == null)
        {
            _logger.LogWarning("Received null general email message");
            return;
        }
        
        _logger.LogInformation("Processing general email for {Email}", message.ToEmail);
        await SendEmailAsync(
            message.ToEmail,
            message.Subject,
            message.HtmlBody,
            cancellationToken
        );
    }

    private async Task SendEmailAsync(
        string toEmail, 
        string subject, 
        string htmlBody, 
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                Timeout = 30000 
            };

            _logger.LogInformation("Sending email to {Email}, setup took {SetupTimeMs}ms", toEmail, stopwatch.ElapsedMilliseconds);
            var smtpStopwatch = Stopwatch.StartNew();
            
            await client.SendMailAsync(message, cancellationToken);
            
            _logger.LogInformation("Email sent successfully to {Email}, SMTP operation took {SmtpTimeMs}ms, total time {TotalTimeMs}ms", 
                toEmail, smtpStopwatch.ElapsedMilliseconds, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email} after {ElapsedMs}ms", toEmail, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email queue consumer is stopping");
        
        _channel?.Close();
        _connection?.Close();
        
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}