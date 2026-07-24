using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Favorites.Commands;

public record ToggleFavoriteCommand(int UserId, int ProductId) : IRequest<Result<bool, FavoriteError>>;