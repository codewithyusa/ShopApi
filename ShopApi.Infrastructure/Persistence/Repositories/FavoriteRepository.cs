using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Repositories;

public class FavoriteRepository(ShopDbContext context) : IFavoriteRepository
{
    public Task<List<Favorite>> GetByUserIdAsync(int userId, CancellationToken ct) =>
        context.Favorites
            .Include(f => f.Product)
            .Where(f => f.UserId == userId)
            .ToListAsync(ct);

    public Task<Favorite?> GetAsync(int userId, int productId, CancellationToken ct) =>
        context.Favorites.FirstOrDefaultAsync(
            f => f.UserId == userId && f.ProductId == productId, ct);

    public async Task AddAsync(Favorite favorite, CancellationToken ct) =>
        await context.Favorites.AddAsync(favorite, ct);

    public Task RemoveAsync(Favorite favorite, CancellationToken ct)
    {
        context.Favorites.Remove(favorite);
        return Task.CompletedTask;
    }

    public async Task DeleteByProductIdAsync(int productId, CancellationToken ct) =>
        await context.Favorites
            .Where(f => f.ProductId == productId)
            .ExecuteDeleteAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}