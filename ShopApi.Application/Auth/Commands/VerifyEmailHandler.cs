using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class VerifyEmailHandler(IUserRepository users)
    : IRequestHandler<VerifyEmailCommand, Result<bool, EmailVerificationError>>
{
    public async Task<Result<bool, EmailVerificationError>> Handle(
        VerifyEmailCommand command, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(command.UserId, ct);
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
