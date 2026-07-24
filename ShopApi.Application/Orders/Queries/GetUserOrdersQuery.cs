using MediatR;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Orders.Queries;

public record GetUserOrdersQuery(int UserId) : IRequest<List<OrderResponseDto>>;