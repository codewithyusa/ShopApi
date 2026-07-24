using MediatR;
using ShopApi.Application.Cart.Dtos;
using ShopApi.Application.Cart.Queries;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Cart.Commands;

public class UpdateCartItemHandler(
    ICartRepository cart,
    IProductRepository products,
    IMediator mediator)
    : IRequestHandler<UpdateCartItemCommand, Result<CartResponseDto, CartError>>
{
    public async Task<Result<CartResponseDto, CartError>> Handle(
        UpdateCartItemCommand command, CancellationToken ct)
    {
        var item = await cart.GetItemAsync(command.UserId, command.ProductId, ct);
        if (item is null)
            return Result<CartResponseDto, CartError>.Failure(CartError.ItemNotInCart());

        var product = await products.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result<CartResponseDto, CartError>.Failure(CartError.ProductNotFound(command.ProductId));

        if (command.Quantity > product.Stock)
            return Result<CartResponseDto, CartError>.Failure(
                CartError.InsufficientStock(product.Name, product.Stock));

        item.Quantity = command.Quantity;
        await cart.SaveChangesAsync(ct);

        var updatedCart = await mediator.Send(new GetCartQuery(command.UserId), ct);
        return Result<CartResponseDto, CartError>.Success(updatedCart);
    }
}