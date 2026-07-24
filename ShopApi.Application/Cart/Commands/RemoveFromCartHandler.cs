using MediatR;
using ShopApi.Application.Cart.Dtos;
using ShopApi.Application.Cart.Queries;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Cart.Commands;

public class RemoveFromCartHandler(ICartRepository cart, IMediator mediator)
    : IRequestHandler<RemoveFromCartCommand, Result<CartResponseDto, CartError>>
{
    public async Task<Result<CartResponseDto, CartError>> Handle(
        RemoveFromCartCommand command, CancellationToken ct)
    {
        var item = await cart.GetItemAsync(command.UserId, command.ProductId, ct);
        if (item is null)
            return Result<CartResponseDto, CartError>.Failure(CartError.ItemNotInCart());

        await cart.RemoveAsync(item, ct);
        await cart.SaveChangesAsync(ct);

        var updatedCart = await mediator.Send(new GetCartQuery(command.UserId), ct);
        return Result<CartResponseDto, CartError>.Success(updatedCart);
    }
}