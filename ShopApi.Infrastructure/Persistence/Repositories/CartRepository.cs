using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Repositories;

public class CartRepository(ShopDbContext context) : ICartRepository
{
    public Task<List<CartItem>> GetByUserIdAsync(int userId, CancellationToken ct) =>
        context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync(ct);

    public Task<CartItem?> GetItemAsync(int userId, int productId, CancellationToken ct) =>
        context.CartItems.FirstOrDefaultAsync(
            c => c.UserId == userId && c.ProductId == productId, ct);

    public async Task AddAsync(CartItem item, CancellationToken ct) =>
        await context.CartItems.AddAsync(item, ct);

    public Task RemoveAsync(CartItem item, CancellationToken ct)
    {
        context.CartItems.Remove(item);
        return Task.CompletedTask;
    }

    public async Task DeleteByProductIdAsync(int productId, CancellationToken ct) =>
        await context.CartItems
            .Where(c => c.ProductId == productId)
            .ExecuteDeleteAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}