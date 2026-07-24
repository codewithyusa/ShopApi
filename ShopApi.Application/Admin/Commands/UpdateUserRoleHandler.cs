using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Admin.Commands;

public class UpdateUserRoleHandler(IUserRepository users)
    : IRequestHandler<UpdateUserRoleCommand, Result<UserResponseDto, AuthError>>
{
    public async Task<Result<UserResponseDto, AuthError>> Handle(
        UpdateUserRoleCommand command, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result<UserResponseDto, AuthError>.Failure(AuthError.UserNotFound());

        user.Role = command.Role;
        user.UpdatedAt = DateTime.UtcNow;
        await users.SaveChangesAsync(ct);

        return Result<UserResponseDto, AuthError>.Success(
            new UserResponseDto(user.Id, user.Name, user.Email, user.Phone, user.Role));
    }
}