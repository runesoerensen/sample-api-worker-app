using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(Environment.GetEnvironmentVariable("CLOUDAMQP_URL") ?? "amqp://guest:guest@localhost:5672")
    };
    return factory.CreateConnectionAsync().Result;
});
var app = builder.Build();

app.MapPost("/send-email", async (HttpContext context, IConnection rabbitMqConnection) =>
{
    var emailRequest = await JsonSerializer.DeserializeAsync<EmailRequest>(context.Request.Body);
    if (emailRequest == null)
    {
        return Results.BadRequest();
    }

    using var channel = await rabbitMqConnection.CreateChannelAsync();
    await channel.QueueDeclareAsync(queue: "email_queue", durable: true, exclusive: false, autoDelete: false);

    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(emailRequest));
    await channel.BasicPublishAsync("", "email_queue", body);

    return Results.Accepted();
});

app.Run();

record EmailRequest(string To, string Subject, string Body);
