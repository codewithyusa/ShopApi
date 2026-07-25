namespace ShopApi.Application.Analytics.Dtos;

public record DailySalesDto(DateOnly Date, decimal Revenue, int OrderCount);