using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class ForgotPasswordHandler(IUserRepository users, IEmailService email)
    : IRequestHandler<ForgotPasswordCommand, bool>
{
    public async Task<bool> Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(command.Email, ct);

        // Always return true whether or not the email exists — otherwise this
        // endpoint becomes a way to test which emails are registered.
        if (user is null)
            return true;

        var code = PinGenerator.Generate();
        user.ResetPasswordToken = code;
        user.ResetPasswordExpires = DateTime.UtcNow.AddMinutes(15);
        await users.SaveChangesAsync(ct);

        await email.SendAsync(user.Email, "Reset your password", BuildEmailBody(user.Name, code), ct);

        return true;
    }

    private static string BuildEmailBody(string name, string code) => $"""
        <h2>Hi {name},</h2>
        <p>We received a request to reset your password. Use this code:</p>
        <h1 style="letter-spacing:4px">{code}</h1>
        <p>This code expires in 15 minutes. If you didn't request this, ignore this email.</p>
        """;
}