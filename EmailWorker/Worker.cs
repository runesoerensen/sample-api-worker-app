using Shared.Messaging;
using Shared.Models;

public class Worker(ILogger<Worker> logger, RabbitMqService rabbitMqService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker is listening for email requests...");
        await rabbitMqService.StartConsumingAsync(ProcessEmailRequestAsync);
    }

    private async Task ProcessEmailRequestAsync(EmailRequest emailRequest)
    {
        logger.LogInformation($"Sending email to: {emailRequest.To}");
        await Task.Delay(1000); // Simulate email sending
        logger.LogInformation("Email sent.");
    }
}
