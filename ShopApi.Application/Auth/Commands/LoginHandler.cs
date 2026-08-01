using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Auth.Commands;

public class LoginHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    ITokenService tokens,
    IRefreshTokenStore refreshTokens,
    IMediator mediator)
    : IRequestHandler<LoginCommand, Result<LoginResponseDto, AuthError>>
{
    public async Task<Result<LoginResponseDto, AuthError>> Handle(
        LoginCommand command, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(command.Email, ct);
        if (user is null || !hasher.Verify(command.Password, user.PasswordHash))
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.InvalidCredentials());

        if (!user.IsEmailVerified)
        {
            await mediator.Send(new SendVerificationEmailCommand(user.Id), ct);
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.EmailNotVerified());
        }

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