using MediatR;
using ShopApi.Application.Analytics.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Analytics.Queries;

public class GetCouponUsageHandler(IAnalyticsRepository analytics)
    : IRequestHandler<GetCouponUsageQuery, List<CouponUsageDto>>
{
    public Task<List<CouponUsageDto>> Handle(GetCouponUsageQuery query, CancellationToken ct) =>
        analytics.GetCouponUsageAsync(ct);
}