using MediatR;
using ShopApi.Application.Analytics.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Analytics.Queries;

public class GetAnalyticsSummaryHandler(IAnalyticsRepository analytics)
    : IRequestHandler<GetAnalyticsSummaryQuery, AnalyticsSummaryDto>
{
    public Task<AnalyticsSummaryDto> Handle(GetAnalyticsSummaryQuery query, CancellationToken ct) =>
        analytics.GetSummaryAsync(ct);
}