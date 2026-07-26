using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using ShopApi.Application.Interfaces;

namespace ShopApi.Infrastructure.Services;

public class SmtpOptions
{
    public required string Host { get; init; }
    public int Port { get; init; } = 587;
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string FromEmail { get; init; }
    public string FromName { get; init; } = "ShopApi";
}

public class EmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _opts = options.Value;

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct)
    {
        using var client = new SmtpClient(_opts.Host, _opts.Port)
        {
            Credentials = new NetworkCredential(_opts.Username, _opts.Password),
            EnableSsl = true
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_opts.FromEmail, _opts.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(to);

        await client.SendMailAsync(message, ct);
    }
}