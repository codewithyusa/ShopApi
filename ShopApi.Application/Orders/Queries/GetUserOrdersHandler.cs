using MediatR;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Orders.Commands;
using ShopApi.Application.Orders.Dtos;

namespace ShopApi.Application.Orders.Queries;

public class GetUserOrdersHandler(IOrderRepository orders)
    : IRequestHandler<GetUserOrdersQuery, List<OrderResponseDto>>
{
    public async Task<List<OrderResponseDto>> Handle(GetUserOrdersQuery query, CancellationToken ct)
    {
        var list = await orders.GetByUserIdAsync(query.UserId, ct);
        return list.Select(CreateOrderHandler.ToDto).ToList();
    }
}