using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Payments.Commands;

public record VerifyCheckoutCommand(string TxRef) : IRequest<Result<OrderResponseDto, PaymentError>>;