using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Favorites.Commands;
using ShopApi.Application.Favorites.Queries;

namespace ShopApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/favorites")]
public class FavoritesController(IMediator mediator) : ControllerBase
{
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<IActionResult> GetFavorites(CancellationToken ct) =>
        Ok(await mediator.Send(new GetFavoritesQuery(CurrentUserId), ct));

    [HttpPost("{productId}/toggle")]
    public async Task<IActionResult> Toggle(int productId, CancellationToken ct)
    {
        var result = await mediator.Send(new ToggleFavoriteCommand(CurrentUserId, productId), ct);

        return result.Match<IActionResult>(
            onSuccess: isFavorited => Ok(new { isFavorited }),
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Toggle failed", Detail = error.Message }));
    }
}