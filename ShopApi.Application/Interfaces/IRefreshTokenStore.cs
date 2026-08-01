using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface IRefreshTokenStore
{
    Task CreateAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> GetActiveByHashAsync(string tokenHash, CancellationToken ct);
    Task RevokeAsync(RefreshToken token, string? replacedByTokenHash, CancellationToken ct);
    Task RevokeAllForUserAsync(int userId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}