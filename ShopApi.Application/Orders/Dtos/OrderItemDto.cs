namespace ShopApi.Application.Orders.Dtos;

public record OrderItemDto(int ProductId, string ProductName, string Image, int Quantity, decimal Price);