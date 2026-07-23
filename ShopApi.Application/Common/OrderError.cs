namespace ShopApi.Application.Common;

public sealed record OrderError(string Code, string Message)
{
    public static OrderError CartEmpty() =>
        new("cart_empty", "Cannot create an order from an empty cart.");

    public static OrderError NotFound(int orderId) =>
        new("order_not_found", $"Order {orderId} was not found.");

    public static OrderError CannotCancel(string status) =>
        new("cannot_cancel", $"Order cannot be cancelled while status is '{status}'.");
}