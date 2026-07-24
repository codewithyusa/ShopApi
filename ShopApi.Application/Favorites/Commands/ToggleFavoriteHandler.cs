using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Favorites.Commands;

public class ToggleFavoriteHandler(IFavoriteRepository favorites, IProductRepository products)
    : IRequestHandler<ToggleFavoriteCommand, Result<bool, FavoriteError>>
{
    public async Task<Result<bool, FavoriteError>> Handle(
        ToggleFavoriteCommand command, CancellationToken ct)
    {
        var product = await products.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result<bool, FavoriteError>.Failure(FavoriteError.ProductNotFound(command.ProductId));

        var existing = await favorites.GetAsync(command.UserId, command.ProductId, ct);

        if (existing is not null)
        {
            await favorites.RemoveAsync(existing, ct);
            await favorites.SaveChangesAsync(ct);
            return Result<bool, FavoriteError>.Success(false); // now unfavorited
        }

        await favorites.AddAsync(new Favorite
        {
            UserId = command.UserId,
            ProductId = command.ProductId
        }, ct);
        await favorites.SaveChangesAsync(ct);

        return Result<bool, FavoriteError>.Success(true); // now favorited
    }
}