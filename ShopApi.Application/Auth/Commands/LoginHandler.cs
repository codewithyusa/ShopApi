using MediatR;
using Microsoft.AspNetCore.Identity;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Auth.Commands;

public class LoginHandler(
    UserManager<User> userManager,
    ITokenService tokens,
    IRefreshTokenStore refreshTokens)
    : IRequestHandler<LoginCommand, Result<LoginResponseDto, AuthError>>
{
    public async Task<Result<LoginResponseDto, AuthError>> Handle(
        LoginCommand command, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.InvalidCredentials());

        // Lockout check
        if (await userManager.IsLockedOutAsync(user))
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.AccountLocked());

        // Password check
        var validPassword = await userManager.CheckPasswordAsync(user, command.Password);
        if (!validPassword)
        {
            await userManager.AccessFailedAsync(user); // increment failed count
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.InvalidCredentials());
        }

        // Reset failed count on success
        await userManager.ResetAccessFailedCountAsync(user);

        if (!user.IsEmailVerified)
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.EmailNotVerified());

        var accessToken = tokens.GenerateAccessToken(user);
        var (rawRefreshToken, expiresAt) = tokens.GenerateRefreshToken();

        await refreshTokens.CreateAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokens.HashToken(rawRefreshToken),
            ExpiresAt = expiresAt
        }, ct);
        await refreshTokens.SaveChangesAsync(ct);

        var dto = new UserResponseDto(user.Id, user.Name, user.Email, user.Phone, user.Role);

        return Result<LoginResponseDto, AuthError>.Success(
            new LoginResponseDto(accessToken, dto)
            {
                RefreshToken = rawRefreshToken,
                RefreshTokenExpiresAt = expiresAt
            });
    }
}