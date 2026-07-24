using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Admin.Commands;

public class DeleteUserHandler(IUserRepository users)
    : IRequestHandler<DeleteUserCommand, Result<bool, AuthError>>
{
    public async Task<Result<bool, AuthError>> Handle(DeleteUserCommand command, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result<bool, AuthError>.Failure(AuthError.UserNotFound());

        await users.DeleteAsync(user, ct);
        await users.SaveChangesAsync(ct);

        return Result<bool, AuthError>.Success(true);
    }
}