using MediatR;
using ShopApi.Application.Interfaces;
using ShopApi.Infrastructure.Services; // PinGenerator — same Application→Infrastructure caveat as SendVerificationEmailHandler
using ShopApi.Infrastructure.Services.EmailTemplates;

namespace ShopApi.Application.Auth.Commands;

public class ForgotPasswordHandler(IUserRepository users, IEmailService email)
    : IRequestHandler<ForgotPasswordCommand, bool>
{
    public async Task<bool> Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(command.Email, ct);

        // Always return the same result whether or not the email exists —
        // otherwise this endpoint becomes a way to test which emails are registered.
        if (user is null)
            return true;

        var code = PinGenerator.Generate();
        user.ResetPasswordToken = code;
        user.ResetPasswordExpires = DateTime.UtcNow.AddMinutes(15);
        await users.SaveChangesAsync(ct);

        await email.SendAsync(
            user.Email, "Reset your password",
            PasswordResetEmailTemplate.Build(user.Name, code), ct);

        return true;
    }
}