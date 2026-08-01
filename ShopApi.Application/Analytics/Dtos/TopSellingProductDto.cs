namespace ShopApi.Application.Analytics.Dtos;

public record TopSellingProductDto(
    int ProductId,
    string Name,
    string Image,
    int TotalQuantitySold,
    decimal TotalRevenue);