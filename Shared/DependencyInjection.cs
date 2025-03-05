using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shared.Messaging;

namespace Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        return services.AddSingleton<IConnectionFactory>(services =>
        {
            return new ConnectionFactory
            {
                Uri = new Uri(services.GetRequiredService<IConfiguration>().GetConnectionString("CloudAMQP")!),
            };
        })
        .AddTransient<RabbitMqService>();
    }
}
