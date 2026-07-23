using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface ICouponRepository
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct);
    Task<Coupon?> GetActiveForUserAsync(int userId, CancellationToken ct);
    Task AddAsync(Coupon coupon, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}