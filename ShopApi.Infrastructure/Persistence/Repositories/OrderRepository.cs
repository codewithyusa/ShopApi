using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Common;
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

    public async Task<(List<Order> Items, int TotalCount)> GetAllPagedAsync(
        PagedRequest request, CancellationToken ct)
    {
        var query = context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public Task<Order?> GetByPaymentRefAsync(string paymentRef, CancellationToken ct) =>
        context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.PaymentRef == paymentRef, ct);

    public async Task AddAsync(Order order, CancellationToken ct) =>
        await context.Orders.AddAsync(order, ct);

    public Task SaveChangesAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}