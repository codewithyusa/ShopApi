using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Orders.Commands;

public record CancelOrderCommand(int UserId, int OrderId) : IRequest<Result<OrderResponseDto, OrderError>>;