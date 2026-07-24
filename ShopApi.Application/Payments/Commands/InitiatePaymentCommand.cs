using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Payments.Commands;

public record InitiatePaymentCommand(int UserId, int OrderId) : IRequest<Result<string, PaymentError>>;