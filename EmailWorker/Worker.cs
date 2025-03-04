using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmailWorker;

public class Worker(ILogger<Worker> logger, IConnection connection) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: "email_queue", durable: true, exclusive: false, autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var emailRequest = JsonSerializer.Deserialize<EmailRequest>(message);

            if (emailRequest != null)
            {
                logger.LogInformation($"Sending email to: {emailRequest.To}");
                await Task.Delay(1000); // Simulate email sending
                logger.LogInformation("Email sent.");
            }

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        };

        await channel.BasicConsumeAsync(queue: "email_queue", autoAck: false, consumer: consumer);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
        logger.LogInformation("Worker is stopping.");
    }
}

record EmailRequest(string To, string Subject, string Body);
