using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Infrastructure.Services;
using ShopApi.Infrastructure.Services.EmailTemplates;

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

        await email.SendAsync(
            user.Email,
            "Verify your email",
            VerificationEmailTemplate.Build(user.Name, code),
            ct);

        return Result<bool, EmailVerificationError>.Success(true);
    }
}
