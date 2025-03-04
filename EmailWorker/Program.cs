using Shared;

var builder = Host.CreateApplicationBuilder(args);

var cloudAmqpUrl = Environment.GetEnvironmentVariable("CLOUDAMQP_URL");
if (!string.IsNullOrEmpty(cloudAmqpUrl))
{
    builder.Configuration["ConnectionStrings:CloudAMQP"] = cloudAmqpUrl;
}

builder.Services.AddMessaging();
builder.Services.AddHostedService<Worker>();

await builder.Build().RunAsync();
