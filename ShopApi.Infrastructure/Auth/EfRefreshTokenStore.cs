using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;
using ShopApi.Infrastructure.Persistence;

namespace ShopApi.Infrastructure.Auth;

public class EfRefreshTokenStore(ShopDbContext context) : IRefreshTokenStore
{
    public async Task CreateAsync(RefreshToken token, CancellationToken ct) =>
        await context.RefreshTokens.AddAsync(token, ct);

    public Task<RefreshToken?> GetActiveByHashAsync(string tokenHash, CancellationToken ct) =>
        context.RefreshTokens.FirstOrDefaultAsync(r =>
            r.TokenHash == tokenHash && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow, ct);

    public Task RevokeAsync(RefreshToken token, string? replacedByTokenHash, CancellationToken ct)
    {
        token.RevokedAt = DateTime.UtcNow;
        token.ReplacedByTokenHash = replacedByTokenHash;
        return Task.CompletedTask;
    }

    public async Task RevokeAllForUserAsync(int userId, CancellationToken ct)
    {
        var active = await context.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in active)
            token.RevokedAt = DateTime.UtcNow;
    }

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}