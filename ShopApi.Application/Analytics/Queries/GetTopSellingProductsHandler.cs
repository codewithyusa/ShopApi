using MediatR;
using ShopApi.Application.Analytics.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Analytics.Queries;

public class GetTopSellingProductsHandler(IAnalyticsRepository analytics)
    : IRequestHandler<GetTopSellingProductsQuery, List<TopSellingProductDto>>
{
    public Task<List<TopSellingProductDto>> Handle(GetTopSellingProductsQuery query, CancellationToken ct) =>
        analytics.GetTopSellingProductsAsync(query.TopN, ct);
}