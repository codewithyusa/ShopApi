using ShopApi.Application.Analytics.Dtos;

namespace ShopApi.Application.Interfaces;

public interface IAnalyticsRepository
{
    Task<AnalyticsSummaryDto> GetSummaryAsync(CancellationToken ct);
    Task<List<DailySalesDto>> GetDailySalesAsync(int days, CancellationToken ct);
}