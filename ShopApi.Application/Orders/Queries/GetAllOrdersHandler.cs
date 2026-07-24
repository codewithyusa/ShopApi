using MediatR;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Orders.Commands;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Orders.Queries;

public class GetAllOrdersHandler(IOrderRepository orders)
    : IRequestHandler<GetAllOrdersQuery, List<OrderResponseDto>>
{
    public async Task<List<OrderResponseDto>> Handle(GetAllOrdersQuery query, CancellationToken ct)
    {
        var list = await orders.GetAllAsync(ct);
        return list.Select(CreateOrderHandler.ToDto).ToList();
    }
}