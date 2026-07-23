using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Repositories;

public class CouponRepository(ShopDbContext context) : ICouponRepository
{
    public Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct) =>
        context.Coupons.FirstOrDefaultAsync(c => c.Code == code, ct);

    public Task<Coupon?> GetActiveForUserAsync(int userId, CancellationToken ct) =>
        context.Coupons.FirstOrDefaultAsync(
            c => c.UserId == userId && c.IsActive && c.ExpirationDate > DateTime.UtcNow, ct);

    public async Task AddAsync(Coupon coupon, CancellationToken ct) =>
        await context.Coupons.AddAsync(coupon, ct);

    public Task SaveChangesAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}