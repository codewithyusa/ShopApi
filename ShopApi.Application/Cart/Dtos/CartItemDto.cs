namespace ShopApi.Application.Cart.Dtos;

public record CartItemDto(int Id, int ProductId, string ProductName, decimal Price, string Image, int Quantity);