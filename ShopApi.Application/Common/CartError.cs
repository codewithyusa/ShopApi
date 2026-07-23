namespace ShopApi.Application.Common;

public sealed record CartError(string Code, string Message)
{
    public static CartError ProductNotFound(int productId) =>
        new("product_not_found", $"Product {productId} was not found.");

    public static CartError InsufficientStock(string productName, int available) =>
        new("insufficient_stock", $"'{productName}' only has {available} in stock.");

    public static CartError ItemNotInCart() =>
        new("item_not_in_cart", "This item is not in the cart.");
}