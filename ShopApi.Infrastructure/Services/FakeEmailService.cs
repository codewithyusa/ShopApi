using ShopApi.Application.Interfaces;

namespace ShopApi.Infrastructure.Services;

/// <summary>
/// Stub implementation used until real SMTP credentials are available.
/// Logs the email to console instead of actually sending it.
/// </summary>
public class FakeEmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct)
    {
        Console.WriteLine("=== FAKE EMAIL ===");
        Console.WriteLine($"To: {to}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Body: {htmlBody}");
        Console.WriteLine("==================");
        return Task.CompletedTask;
    }
}