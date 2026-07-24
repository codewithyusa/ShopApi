using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Orders.Commands;

public record CreateOrderCommand(int UserId) : IRequest<Result<OrderResponseDto, OrderError>>;