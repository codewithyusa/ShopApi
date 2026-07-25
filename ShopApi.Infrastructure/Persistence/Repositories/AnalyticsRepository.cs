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
}