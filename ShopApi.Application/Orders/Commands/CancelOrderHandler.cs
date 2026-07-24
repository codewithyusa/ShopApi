using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Orders.Commands;

public class CancelOrderHandler(IOrderRepository orders)
    : IRequestHandler<CancelOrderCommand, Result<OrderResponseDto, OrderError>>
{
    public async Task<Result<OrderResponseDto, OrderError>> Handle(
        CancelOrderCommand command, CancellationToken ct)
    {
        var order = await orders.GetByIdAsync(command.OrderId, ct);
        if (order is null || order.UserId != command.UserId)
            return Result<OrderResponseDto, OrderError>.Failure(OrderError.NotFound(command.OrderId));

        if (order.OrderStatus is "shipped" or "delivered" or "cancelled")
            return Result<OrderResponseDto, OrderError>.Failure(OrderError.CannotCancel(order.OrderStatus));

        order.OrderStatus = "cancelled";
        order.UpdatedAt = DateTime.UtcNow;
        await orders.SaveChangesAsync(ct);

        return Result<OrderResponseDto, OrderError>.Success(CreateOrderHandler.ToDto(order));
    }
}