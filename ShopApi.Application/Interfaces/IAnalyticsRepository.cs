using ShopApi.Application.Analytics.Dtos;

namespace ShopApi.Application.Interfaces;

public interface IAnalyticsRepository
{
    Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken ct);
    Task<List<DailySalesDto>> GetDailySalesAsync(int days, CancellationToken ct);
    Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int topN, CancellationToken ct);
    Task<List<CouponUsageDto>> GetCouponUsageAsync(CancellationToken ct);
}