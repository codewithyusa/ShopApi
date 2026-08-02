using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Orders.Queries;

public record GetAllOrdersQuery(PagedRequest Paging) : IRequest<PagedResponse<OrderResponseDto>>;