namespace ShopApi.Application.Analytics.Dtos;

public record AnalyticsSummaryDto(
    decimal TotalRevenue,
    int TotalOrders,
    int PendingOrders,
    int TotalUsers,
    int TotalProducts,
    decimal AverageOrderValue);