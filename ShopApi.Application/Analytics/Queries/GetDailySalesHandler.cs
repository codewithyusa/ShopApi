using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Analytics.Dtos;
using ShopApi.Infrastructure.Persistence;

namespace ShopApi.Application.Analytics.Queries;

public class GetDailySalesHandler(ShopDbContext context)
    : IRequestHandler<GetDailySalesQuery, List<DailySalesDto>>
{
    public async Task<List<DailySalesDto>> Handle(GetDailySalesQuery query, CancellationToken ct)
    {
        var since = DateTime.UtcNow.Date.AddDays(-query.Days);

        var raw = await context.Orders
            .Where(o => o.PaymentStatus == "paid" && o.CreatedAt >= since)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount), Count = g.Count() })
            .ToListAsync(ct);

        // Fill in zero-sales days too, so the chart on the frontend doesn't have gaps.
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