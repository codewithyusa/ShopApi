using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Orders.Commands;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Payments.Commands;

public class VerifyCheckoutHandler(IOrderRepository orders, IChapaPaymentService chapa)
    : IRequestHandler<VerifyCheckoutCommand, Result<OrderResponseDto, PaymentError>>
{
    public async Task<Result<OrderResponseDto, PaymentError>> Handle(
        VerifyCheckoutCommand command, CancellationToken ct)
    {
        var result = await chapa.VerifyAsync(command.TxRef, ct);
        if (!result.Success)
            return Result<OrderResponseDto, PaymentError>.Failure(
                PaymentError.VerificationFailed(command.TxRef));

        // Find the order by its stored PaymentRef (set during InitiatePayment).
        var allOrders = await orders.GetAllAsync(ct);
        var order = allOrders.FirstOrDefault(o => o.PaymentRef == command.TxRef);

        if (order is null)
            return Result<OrderResponseDto, PaymentError>.Failure(
                PaymentError.VerificationFailed(command.TxRef));

        order.PaymentStatus = result.Status == "success" ? "paid" : "failed";
        if (order.PaymentStatus == "paid")
            order.OrderStatus = "processing";

        order.UpdatedAt = DateTime.UtcNow;
        await orders.SaveChangesAsync(ct);

        return Result<OrderResponseDto, PaymentError>.Success(CreateOrderHandler.ToDto(order));
    }
}