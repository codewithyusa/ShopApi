namespace ShopApi.Application.Products.Dtos;

public record ProductResponseDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    string Image,
    string Category,
    string Color,
    string Size,
    int Stock,
    bool IsFeatured);