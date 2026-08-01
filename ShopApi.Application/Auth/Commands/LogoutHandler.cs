using MediatR;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class LogoutHandler(IRefreshTokenStore refreshTokens, ITokenService tokens)
    : IRequestHandler<LogoutCommand, bool>
{
    public async Task<bool> Handle(LogoutCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.RawRefreshToken))
            return true; // nothing to revoke — treat as already logged out

        var hash = tokens.HashToken(command.RawRefreshToken);
        var existing = await refreshTokens.GetActiveByHashAsync(hash, ct);
        if (existing is not null)
        {
            await refreshTokens.RevokeAsync(existing, null, ct);
            await refreshTokens.SaveChangesAsync(ct);
        }

        return true;
    }
}