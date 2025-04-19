using System.ComponentModel;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;
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
builder.Services.AddMcpServer().WithHttpTransport().WithTools<SampleEmailTool>();
var app = builder.Build();

app.MapPost("/send-email", async (HttpContext context, RabbitMqService rabbitMqService, [FromBody] EmailRequest emailRequest) =>
{
    await rabbitMqService.PublishAsync(emailRequest);
    app.Logger.LogInformation("Email request queued");

    return Results.Accepted();
});

app.MapMcp();
app.Run();

[McpServerToolType]
public sealed class SampleEmailTool
{
    [McpServerTool, Description("Send an email.")]
    public static async Task<string> SendEmail(
        RabbitMqService rabbitMqService,
        [Description("The recipient email address")] string to,
        [Description("The email subject")] string subject,
        [Description("The email body")] string body)
    {
        var emailRequest = new EmailRequest(to, subject, body);
        await rabbitMqService.PublishAsync(emailRequest);

        return $"Email request queued {emailRequest}";
    }
}
