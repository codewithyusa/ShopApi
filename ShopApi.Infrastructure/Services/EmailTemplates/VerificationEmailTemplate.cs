namespace ShopApi.Infrastructure.Services.EmailTemplates;

public static class VerificationEmailTemplate
{
    public static string Build(string name, string code) => $"""
        <h2>Hi {name},</h2>
        <p>Your verification code is:</p>
        <h1 style="letter-spacing:4px">{code}</h1>
        <p>This code expires in 15 minutes.</p>
        """;
}