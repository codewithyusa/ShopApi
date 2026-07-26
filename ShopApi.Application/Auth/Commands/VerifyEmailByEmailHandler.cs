using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class VerifyEmailByEmailHandler(IUserRepository users)
    : IRequestHandler<VerifyEmailByEmailCommand, Result<bool, EmailVerificationError>>
{
    public async Task<Result<bool, EmailVerificationError>> Handle(
        VerifyEmailByEmailCommand command, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(command.Email, ct);
        if (user is null)
            return Result<bool, EmailVerificationError>.Failure(EmailVerificationError.UserNotFound());

        if (user.IsEmailVerified)
            return Result<bool, EmailVerificationError>.Failure(EmailVerificationError.AlreadyVerified());

        if (user.EmailVerificationCode != command.Code ||
            user.EmailVerificationExpires is null ||
            user.EmailVerificationExpires < DateTime.UtcNow)
            return Result<bool, EmailVerificationError>.Failure(EmailVerificationError.InvalidCode());

        user.IsEmailVerified = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationExpires = null;
        user.UpdatedAt = DateTime.UtcNow;
        await users.SaveChangesAsync(ct);

        return Result<bool, EmailVerificationError>.Success(true);
    }
}