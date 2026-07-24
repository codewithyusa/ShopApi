namespace ShopApi.Application.Common;

public sealed record ProductError(string Code, string Message)
{
    public static ProductError NotFound(int id) =>
        new("product_not_found", $"Product {id} was not found.");
}