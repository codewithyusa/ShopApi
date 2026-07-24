using MediatR;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Orders.Queries;

public record GetAllOrdersQuery : IRequest<List<OrderResponseDto>>;