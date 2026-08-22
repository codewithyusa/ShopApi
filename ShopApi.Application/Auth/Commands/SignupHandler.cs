using MediatR;
using Microsoft.AspNetCore.Identity;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Auth.Commands;

public class SignupHandler(UserManager<User> userManager)
    : IRequestHandler<SignupCommand, Result<UserResponseDto, AuthError>>
{
    public async Task<Result<UserResponseDto, AuthError>> Handle(
        SignupCommand command, CancellationToken ct)
    {
        var existing = await userManager.FindByEmailAsync(command.Email);
        if (existing != null)
            return Result<UserResponseDto, AuthError>.Failure(
                AuthError.EmailAlreadyExists(command.Email));

        var role = command.Role == "admin" ? "admin" : "customer";

        var user = new User
        {
            Name = command.Name,
            UserName = command.Email,
            Email = command.Email,
            Phone = command.Phone,
            Role = role,
            IsEmailVerified = true
        };

        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            var error = result.Errors.First().Description;
            return Result<UserResponseDto, AuthError>.Failure(
                AuthError.InvalidCredentials(error));
        }

        return Result<UserResponseDto, AuthError>.Success(
            new UserResponseDto(user.Id, user.Name, user.Email, user.Phone, user.Role));
    }
}