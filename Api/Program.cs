using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Shared;
using Shared.Messaging;
using Shared.Models;

var builder = WebApplication.CreateBuilder(args);
var cloudAmqpUrl = Environment.GetEnvironmentVariable("CLOUDAMQP_URL");
if (!string.IsNullOrEmpty(cloudAmqpUrl))
{
    builder.Configuration["ConnectionStrings:CloudAMQP"] = cloudAmqpUrl;
}
builder.Services.AddMessaging();
var app = builder.Build();

app.MapPost("/send-email", async (HttpContext context, RabbitMqService rabbitMqService, [FromBody] EmailRequest emailRequest) =>
{
    await rabbitMqService.PublishAsync(emailRequest);
    app.Logger.LogInformation("Email request queued");

    return Results.Accepted();
});

app.Run();
