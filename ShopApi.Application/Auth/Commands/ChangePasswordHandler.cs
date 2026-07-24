using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class ChangePasswordHandler(IUserRepository users, IPasswordHasher hasher)
    : IRequestHandler<ChangePasswordCommand, Result<bool, AuthError>>
{
    public async Task<Result<bool, AuthError>> Handle(
        ChangePasswordCommand command, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result<bool, AuthError>.Failure(AuthError.UserNotFound());

        if (!hasher.Verify(command.CurrentPassword, user.PasswordHash))
            return Result<bool, AuthError>.Failure(AuthError.InvalidCredentials());

        user.PasswordHash = hasher.Hash(command.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await users.SaveChangesAsync(ct);

        return Result<bool, AuthError>.Success(true);
    }
}