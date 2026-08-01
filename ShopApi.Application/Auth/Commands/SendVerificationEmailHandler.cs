using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class SendVerificationEmailHandler(IUserRepository users, IEmailService email)
    : IRequestHandler<SendVerificationEmailCommand, Result<bool, EmailVerificationError>>
{
    public async Task<Result<bool, EmailVerificationError>> Handle(
        SendVerificationEmailCommand command, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result<bool, EmailVerificationError>.Failure(EmailVerificationError.UserNotFound());

        if (user.IsEmailVerified)
            return Result<bool, EmailVerificationError>.Failure(EmailVerificationError.AlreadyVerified());

        var code = PinGenerator.Generate();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpires = DateTime.UtcNow.AddMinutes(15);
        await users.SaveChangesAsync(ct);

        await email.SendAsync(user.Email, "Verify your email", BuildEmailBody(user.Name, code), ct);

        return Result<bool, EmailVerificationError>.Success(true);
    }

    private static string BuildEmailBody(string name, string code) => $"""
        <h2>Hi {name},</h2>
        <p>Your verification code is:</p>
        <h1 style="letter-spacing:4px">{code}</h1>
        <p>This code expires in 15 minutes.</p>
        """;
}