using EmailWorker;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(Environment.GetEnvironmentVariable("CLOUDAMQP_URL") ?? "amqp://guest:guest@localhost:5672")
    };
    return factory.CreateConnectionAsync().Result;
});

var host = builder.Build();
host.Run();
