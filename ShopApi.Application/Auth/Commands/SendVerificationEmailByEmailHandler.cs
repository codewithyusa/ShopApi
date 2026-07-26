using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class SendVerificationEmailByEmailHandler(IUserRepository users, IEmailService email)
    : IRequestHandler<SendVerificationEmailByEmailCommand, Result<bool, EmailVerificationError>>
{
    public async Task<Result<bool, EmailVerificationError>> Handle(
        SendVerificationEmailByEmailCommand command, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(command.Email, ct);
        if (user is null)
            return Result<bool, EmailVerificationError>.Failure(EmailVerificationError.UserNotFound());

        if (user.IsEmailVerified)
            return Result<bool, EmailVerificationError>.Failure(EmailVerificationError.AlreadyVerified());

        var code = PinGenerator.Generate();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpires = DateTime.UtcNow.AddMinutes(15);
        await users.SaveChangesAsync(ct);

        await email.SendAsync(
            user.Email,
            "Verify your email",
            VerificationEmailTemplate.Build(user.Name, code),
            ct);

        return Result<bool, EmailVerificationError>.Success(true);
    }
}