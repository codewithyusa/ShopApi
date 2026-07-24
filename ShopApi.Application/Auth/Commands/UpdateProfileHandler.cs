using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class UpdateProfileHandler(IUserRepository users)
    : IRequestHandler<UpdateProfileCommand, Result<UserResponseDto, AuthError>>
{
    public async Task<Result<UserResponseDto, AuthError>> Handle(
        UpdateProfileCommand command, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result<UserResponseDto, AuthError>.Failure(AuthError.UserNotFound());

        user.Name = command.Name;
        user.Phone = command.Phone;
        user.UpdatedAt = DateTime.UtcNow;

        await users.SaveChangesAsync(ct);

        return Result<UserResponseDto, AuthError>.Success(
            new UserResponseDto(user.Id, user.Name, user.Email, user.Phone, user.Role));
    }
}