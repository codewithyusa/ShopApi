using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Orders.Dtos;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Orders.Commands;

public class CreateOrderHandler(
    ICartRepository cart,
    IProductRepository products,
    IOrderRepository orders)
    : IRequestHandler<CreateOrderCommand, Result<OrderResponseDto, OrderError>>
{
    public async Task<Result<OrderResponseDto, OrderError>> Handle(
        CreateOrderCommand command, CancellationToken ct)
    {
        var cartItems = await cart.GetByUserIdAsync(command.UserId, ct);
        if (cartItems.Count == 0)
            return Result<OrderResponseDto, OrderError>.Failure(OrderError.CartEmpty());

        // Re-validate stock at checkout time — it may have changed since being added to cart.
        foreach (var item in cartItems)
        {
            var product = await products.GetByIdAsync(item.ProductId, ct);
            if (product is null || product.Stock < item.Quantity)
                return Result<OrderResponseDto, OrderError>.Failure(OrderError.CartEmpty());
        }

        var orderItems = new List<OrderItem>();
        decimal total = 0;

        foreach (var item in cartItems)
        {
            var product = await products.GetByIdAsync(item.ProductId, ct);
            product!.Stock -= item.Quantity; // tracked entity — EF picks up this mutation on SaveChanges

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price // snapshot price at time of purchase
            });

            total += product.Price * item.Quantity;
        }

        var order = new Order
        {
            UserId = command.UserId,
            TotalAmount = total,
            Items = orderItems
        };

        await orders.AddAsync(order, ct);

        foreach (var item in cartItems)
            await cart.RemoveAsync(item, ct);

        // Single save — atomic across order creation, stock decrement, and cart clearing.
        await orders.SaveChangesAsync(ct);

        return Result<OrderResponseDto, OrderError>.Success(ToDto(order));
    }

    public static OrderResponseDto ToDto(Order order) => new(
        order.Id, order.TotalAmount, order.PaymentStatus, order.OrderStatus, order.CreatedAt,
        order.Items.Select(i => new OrderItemDto(
            i.ProductId, i.Product?.Name ?? "", i.Product?.Image ?? "", i.Quantity, i.Price)).ToList());
}