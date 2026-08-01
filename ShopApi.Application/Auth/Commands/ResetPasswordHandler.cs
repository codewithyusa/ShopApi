using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class ResetPasswordHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    IRefreshTokenStore refreshTokens)
    : IRequestHandler<ResetPasswordCommand, Result<bool, AuthError>>
{
    public async Task<Result<bool, AuthError>> Handle(
        ResetPasswordCommand command, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(command.Email, ct);
        if (user is null)
            return Result<bool, AuthError>.Failure(AuthError.InvalidResetToken());

        if (user.ResetPasswordToken != command.Code ||
            user.ResetPasswordExpires is null ||
            user.ResetPasswordExpires < DateTime.UtcNow)
            return Result<bool, AuthError>.Failure(AuthError.InvalidResetToken());

        user.PasswordHash = hasher.Hash(command.NewPassword);
        user.ResetPasswordToken = null;
        user.ResetPasswordExpires = null;
        user.UpdatedAt = DateTime.UtcNow;
        await users.SaveChangesAsync(ct);

        // A password reset should kill every existing session — otherwise a refresh
        // token issued before the reset still works after it.
        await refreshTokens.RevokeAllForUserAsync(user.Id, ct);
        await refreshTokens.SaveChangesAsync(ct);

        return Result<bool, AuthError>.Success(true);
    }
}