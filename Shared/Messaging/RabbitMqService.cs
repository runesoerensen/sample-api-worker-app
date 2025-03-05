using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;

namespace Shared.Messaging;

public class RabbitMqService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queueName;

    public RabbitMqService(IConnectionFactory factory)
    {
        _connection = factory.CreateConnectionAsync().Result;
        _channel = _connection.CreateChannelAsync().Result;

        _queueName = "email_queue";

        _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false);
    }

    public async Task PublishAsync(EmailRequest emailRequest)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(emailRequest));

        await _channel.BasicPublishAsync("", _queueName, body);
    }

    public async Task StartConsumingAsync(Func<EmailRequest, Task> processMessageAsync)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            var emailRequest = JsonSerializer.Deserialize<EmailRequest>(messageJson);

            if (emailRequest != null)
            {
                await processMessageAsync(emailRequest);
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
        };

        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);
    }

    public void Dispose()
    {
        _channel.CloseAsync();
        _connection.CloseAsync();
    }
}
