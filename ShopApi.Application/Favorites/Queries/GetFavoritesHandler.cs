using MediatR;
using ShopApi.Application.Favorites.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Favorites.Queries;

public class GetFavoritesHandler(IFavoriteRepository favorites)
    : IRequestHandler<GetFavoritesQuery, List<FavoriteResponseDto>>
{
    public async Task<List<FavoriteResponseDto>> Handle(GetFavoritesQuery query, CancellationToken ct)
    {
        var list = await favorites.GetByUserIdAsync(query.UserId, ct);
        return list.Select(f => new FavoriteResponseDto(
            f.Id, f.ProductId, f.Product.Name, f.Product.Price, f.Product.Image)).ToList();
    }
}