namespace ShopApi.Application.Favorites.Dtos;

public record FavoriteResponseDto(int Id, int ProductId, string ProductName, decimal Price, string Image);