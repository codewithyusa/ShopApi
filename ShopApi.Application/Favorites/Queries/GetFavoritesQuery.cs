using MediatR;
using ShopApi.Application.Favorites.Dtos;

namespace ShopApi.Application.Favorites.Queries;

public record GetFavoritesQuery(int UserId) : IRequest<List<FavoriteResponseDto>>;