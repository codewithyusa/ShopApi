using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Analytics.Dtos;
using ShopApi.Infrastructure.Persistence;

namespace ShopApi.Application.Analytics.Queries;

public class GetAnalyticsSummaryHandler(ShopDbContext context)
    : IRequestHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryDto>
{
    public async Task<AnalyticsSummaryDto> Handle(GetAnalyticsSummaryQuery query, CancellationToken ct)
    {
        // Only count paid revenue as "real" revenue — pending/failed orders shouldn't inflate this.
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
}