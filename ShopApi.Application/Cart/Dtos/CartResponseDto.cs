namespace ShopApi.Application.Cart.Dtos;

public record CartResponseDto(List<CartItemDto> Items, decimal Total);