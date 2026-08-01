namespace ShopApi.Application.Analytics.Dtos;

public record CouponUsageDto(
    string CouponCode,
    int UsageCount,
    decimal TotalDiscountGiven);