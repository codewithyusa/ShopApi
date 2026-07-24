using MediatR;
using ShopApi.Application.Cart.Dtos;
using ShopApi.Application.Cart.Queries;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Cart.Commands;

public class AddToCartHandler(
    ICartRepository cart,
    IProductRepository products,
    IMediator mediator)
    : IRequestHandler<AddToCartCommand, Result<CartResponseDto, CartError>>
{
    public async Task<Result<CartResponseDto, CartError>> Handle(
        AddToCartCommand command, CancellationToken ct)
    {
        var product = await products.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result<CartResponseDto, CartError>.Failure(CartError.ProductNotFound(command.ProductId));

        var existing = await cart.GetItemAsync(command.UserId, command.ProductId, ct);
        var requestedTotal = (existing?.Quantity ?? 0) + command.Quantity;

        if (requestedTotal > product.Stock)
            return Result<CartResponseDto, CartError>.Failure(
                CartError.InsufficientStock(product.Name, product.Stock));

        if (existing is not null)
        {
            existing.Quantity = requestedTotal;
        }
        else
        {
            await cart.AddAsync(new CartItem
            {
                UserId = command.UserId,
                ProductId = command.ProductId,
                Quantity = command.Quantity
            }, ct);
        }

        await cart.SaveChangesAsync(ct);

        var updatedCart = await mediator.Send(new GetCartQuery(command.UserId), ct);
        return Result<CartResponseDto, CartError>.Success(updatedCart);
    }
}