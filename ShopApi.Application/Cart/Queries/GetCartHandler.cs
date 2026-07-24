using MediatR;
using ShopApi.Application.Cart.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Cart.Queries;

public class GetCartHandler(ICartRepository cart) : IRequestHandler<GetCartQuery, CartResponseDto>
{
    public async Task<CartResponseDto> Handle(GetCartQuery query, CancellationToken ct)
    {
        var items = await cart.GetByUserIdAsync(query.UserId, ct);

        var dtos = items.Select(i => new CartItemDto(
            i.Id, i.ProductId, i.Product.Name, i.Product.Price, i.Product.Image, i.Quantity)).ToList();

        var total = dtos.Sum(i => i.Price * i.Quantity);

        return new CartResponseDto(dtos, total);
    }
}