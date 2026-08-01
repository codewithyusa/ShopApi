using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Auth.Commands;

public class RefreshTokenHandler(
    IRefreshTokenStore refreshTokens,
    IUserRepository users,
    ITokenService tokens)
    : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDto, AuthError>>
{
    public async Task<Result<LoginResponseDto, AuthError>> Handle(
        RefreshTokenCommand command, CancellationToken ct)
    {
        var incomingHash = tokens.HashToken(command.RawRefreshToken);
        var existing = await refreshTokens.GetActiveByHashAsync(incomingHash, ct);
        if (existing is null)
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.InvalidRefreshToken());

        var user = await users.GetByIdAsync(existing.UserId, ct);
        if (user is null)
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.UserNotFound());

        // Rotation: this token is now spent. If a stolen copy of the old cookie
        // gets replayed after this point, it hits a revoked row and fails outright —
        // that mismatch is also your signal to alert the user of a compromised session.
        var (newRaw, newExpiresAt) = tokens.GenerateRefreshToken();
        var newHash = tokens.HashToken(newRaw);

        await refreshTokens.RevokeAsync(existing, newHash, ct);
        await refreshTokens.CreateAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newHash,
            ExpiresAt = newExpiresAt
        }, ct);
        await refreshTokens.SaveChangesAsync(ct);

        var accessToken = tokens.GenerateAccessToken(user);
        var dto = new UserResponseDto(user.Id, user.Name, user.Email, user.Phone, user.Role);

        return Result<LoginResponseDto, AuthError>.Success(
            new LoginResponseDto(accessToken, dto)
            {
                RefreshToken = newRaw,
                RefreshTokenExpiresAt = newExpiresAt
            });
    }
}