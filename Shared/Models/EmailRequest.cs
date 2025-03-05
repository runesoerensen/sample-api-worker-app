using System.Text.Json;

namespace Shared.Models;

public record EmailRequest(string To, string Subject, string Body);
