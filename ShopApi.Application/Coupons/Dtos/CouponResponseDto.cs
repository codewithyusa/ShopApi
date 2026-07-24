namespace ShopApi.Application.Coupons.Dtos;

public record CouponResponseDto(int Id, string Code, decimal DiscountPercentage, DateTime ExpirationDate);