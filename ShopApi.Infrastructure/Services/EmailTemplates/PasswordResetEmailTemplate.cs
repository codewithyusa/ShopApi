namespace ShopApi.Infrastructure.Services.EmailTemplates;

public static class PasswordResetEmailTemplate
{
    public static string Build(string name, string code) => $"""
        <h2>Hi {name},</h2>
        <p>We received a request to reset your password. Use this code:</p>
        <h1 style="letter-spacing:4px">{code}</h1>
        <p>This code expires in 15 minutes. If you didn't request this, ignore this email.</p>
        """;
}