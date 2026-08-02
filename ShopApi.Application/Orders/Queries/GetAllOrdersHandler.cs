using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Orders.Commands;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Orders.Queries;

public class GetAllOrdersHandler(IOrderRepository orders)
    : IRequestHandler<GetAllOrdersQuery, PagedResponse<OrderResponseDto>>
{
    public async Task<PagedResponse<OrderResponseDto>> Handle(GetAllOrdersQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await orders.GetAllPagedAsync(query.Paging, ct);

        return new PagedResponse<OrderResponseDto>
        {
            Items = items.Select(CreateOrderHandler.ToDto).ToList(),
            TotalCount = totalCount,
            Page = query.Paging.Page,
            PageSize = query.Paging.PageSize
        };
    }
}