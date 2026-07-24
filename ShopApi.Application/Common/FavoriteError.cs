namespace ShopApi.Application.Common;

public sealed record FavoriteError(string Code, string Message)
{
    public static FavoriteError ProductNotFound(int productId) =>
        new("product_not_found", $"Product {productId} was not found.");

    public static FavoriteError AlreadyFavorited() =>
        new("already_favorited", "This product is already in your favorites.");

    public static FavoriteError NotFavorited() =>
        new("not_favorited", "This product is not in your favorites.");
}