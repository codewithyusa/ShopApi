using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Auth.Commands;

public class SignupHandler(IUserRepository users, IPasswordHasher hasher)
    : IRequestHandler<SignupCommand, Result<UserResponseDto, AuthError>>
{
    public async Task<Result<UserResponseDto, AuthError>> Handle(
        SignupCommand command, CancellationToken ct)
    {
        if (await users.EmailExistsAsync(command.Email, ct))
            return Result<UserResponseDto, AuthError>.Failure(
                AuthError.EmailAlreadyExists(command.Email));

        var user = new User
        {
            Name = command.Name,
            Email = command.Email,
            PasswordHash = hasher.Hash(command.Password),
            Phone = command.Phone
        };

        await users.AddAsync(user, ct);
        await users.SaveChangesAsync(ct);

        return Result<UserResponseDto, AuthError>.Success(
            new UserResponseDto(user.Id, user.Name, user.Email, user.Phone, user.Role));
    }
}