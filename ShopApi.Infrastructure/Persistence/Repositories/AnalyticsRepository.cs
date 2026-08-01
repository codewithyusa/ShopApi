using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Analytics.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Infrastructure.Persistence.Repositories;

public class AnalyticsRepository(ShopDbContext context) : IAnalyticsRepository
{
    public async Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken ct)
    {
        var paidOrders = context.Orders.Where(o => o.PaymentStatus == "paid");

        var totalRevenue = await paidOrders.SumAsync(o => (decimal?)o.TotalAmount, ct) ?? 0m;
        var totalOrders = await context.Orders.CountAsync(ct);
        var pendingOrders = await context.Orders.CountAsync(o => o.OrderStatus == "pending", ct);
        var totalUsers = await context.Users.CountAsync(ct);
        var totalProducts = await context.Products.CountAsync(ct);

        var paidCount = await paidOrders.CountAsync(ct);
        var averageOrderValue = paidCount == 0 ? 0m : totalRevenue / paidCount;

        return new AnalyticsSummaryDto(
            totalRevenue, totalOrders, pendingOrders, totalUsers, totalProducts, averageOrderValue);
    }

    public async Task<List<DailySalesDto>> GetDailySalesAsync(int days, CancellationToken ct)
    {
        var since = DateTime.UtcNow.Date.AddDays(-days);

        var raw = await context.Orders
            .Where(o => o.PaymentStatus == "paid" && o.CreatedAt >= since)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount), Count = g.Count() })
            .ToListAsync(ct);

        var result = new List<DailySalesDto>();
        for (var day = since; day <= DateTime.UtcNow.Date; day = day.AddDays(1))
        {
            var match = raw.FirstOrDefault(r => r.Date == day);
            result.Add(new DailySalesDto(
                DateOnly.FromDateTime(day), match?.Revenue ?? 0m, match?.Count ?? 0));
        }

        return result;
    }

    public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int topN, CancellationToken ct)
    {
        // Only count items from paid orders — a pending/failed order hasn't actually "sold" anything yet.
        return await context.OrderItems
            .Where(oi => oi.Order.PaymentStatus == "paid")
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name, oi.Product.Image })
            .Select(g => new TopSellingProductDto(
                g.Key.ProductId,
                g.Key.Name,
                g.Key.Image,
                g.Sum(oi => oi.Quantity),
                g.Sum(oi => oi.Price * oi.Quantity)))
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(topN)
            .ToListAsync(ct);
    }

    public async Task<List<CouponUsageDto>> GetCouponUsageAsync(CancellationToken ct)
    {
        // Discount isn't stored directly — it's implied by (line-item subtotal - final TotalAmount).
        // Subtotal is computed in SQL via the projection below; the GroupBy runs in memory
        // afterward since coupon usage tables are small and this keeps the query translatable.
        var raw = await context.Orders
            .Where(o => o.CouponCode != null)
            .Select(o => new
            {
                o.CouponCode,
                o.TotalAmount,
                Subtotal = o.Items.Sum(i => i.Price * i.Quantity)
            })
            .ToListAsync(ct);

        return raw
            .GroupBy(o => o.CouponCode!)
            .Select(g => new CouponUsageDto(
                g.Key,
                g.Count(),
                g.Sum(o => o.Subtotal - o.TotalAmount)))
            .OrderByDescending(c => c.UsageCount)
            .ToList();
    }
}