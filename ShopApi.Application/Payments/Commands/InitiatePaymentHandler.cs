using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Payments.Commands;

public class InitiatePaymentHandler(
    IOrderRepository orders,
    IUserRepository users,
    IChapaPaymentService chapa)
    : IRequestHandler<InitiatePaymentCommand, Result<string, PaymentError>>
{
    public async Task<Result<string, PaymentError>> Handle(
        InitiatePaymentCommand command, CancellationToken ct)
    {
        var order = await orders.GetByIdAsync(command.OrderId, ct);
        if (order is null || order.UserId != command.UserId)
            return Result<string, PaymentError>.Failure(PaymentError.InitiationFailed("Order not found."));

        if (order.PaymentStatus == "paid")
            return Result<string, PaymentError>.Failure(PaymentError.AlreadyPaid());

        var user = await users.GetByIdAsync(command.UserId, ct);
        if (user is null)
            return Result<string, PaymentError>.Failure(PaymentError.InitiationFailed("User not found."));

        // tx_ref must be unique per attempt — embedding order id keeps it traceable back to the order.
        var txRef = $"order-{order.Id}-{Guid.NewGuid():N}";

        var result = await chapa.InitializeAsync(
            txRef, order.TotalAmount, user.Email, user.Name, ct);

        if (!result.Success || result.CheckoutUrl is null)
            return Result<string, PaymentError>.Failure(PaymentError.InitiationFailed(result.Error ?? "Unknown error."));

        order.PaymentRef = txRef;
        await orders.SaveChangesAsync(ct);

        return Result<string, PaymentError>.Success(result.CheckoutUrl);
    }
}