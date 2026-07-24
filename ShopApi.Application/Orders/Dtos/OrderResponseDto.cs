namespace ShopApi.Application.Orders.Dtos;

public record OrderResponseDto(
    int Id,
    decimal TotalAmount,
    string PaymentStatus,
    string OrderStatus,
    DateTime CreatedAt,
    List<OrderItemDto> Items);
    