using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Repositories;

public class OrderRepository(ShopDbContext context) : IOrderRepository
{
    public Task<Order?> GetByIdAsync(int id, CancellationToken ct) =>
        context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken ct) =>
        context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync(ct);

    public Task<List<Order>> GetAllAsync(CancellationToken ct) =>
        context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task AddAsync(Order order, CancellationToken ct) =>
        await context.Orders.AddAsync(order, ct);

    public Task SaveChangesAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}