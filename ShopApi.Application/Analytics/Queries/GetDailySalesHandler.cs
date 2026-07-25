using MediatR;
using ShopApi.Application.Analytics.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Analytics.Queries;

public class GetDailySalesHandler(IAnalyticsRepository analytics)
    : IRequestHandler<GetDailySalesQuery, List<DailySalesDto>>
{
    public Task<List<DailySalesDto>> Handle(GetDailySalesQuery query, CancellationToken ct) =>
        analytics.GetDailySalesAsync(query.Days, ct);
}