using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Cart.Commands;
using ShopApi.Application.Cart.Queries;

namespace ShopApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/cart")]
public class CartController(IMediator mediator) : ControllerBase
{
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct) =>
        Ok(await mediator.Send(new GetCartQuery(CurrentUserId), ct));

    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new AddToCartCommand(CurrentUserId, request.ProductId, request.Quantity), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: MapError);
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateItem(int productId, UpdateCartItemRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateCartItemCommand(CurrentUserId, productId, request.Quantity), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: MapError);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveItem(int productId, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveFromCartCommand(CurrentUserId, productId), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: MapError);
    }

    private IActionResult MapError(Application.Common.CartError error) => error.Code switch
    {
        "insufficient_stock" => Conflict(new ProblemDetails { Status = 409, Title = "Cart update failed", Detail = error.Message }),
        _ => NotFound(new ProblemDetails { Status = 404, Title = "Cart update failed", Detail = error.Message })
    };
}

public record AddToCartRequest(int ProductId, int Quantity);
public record UpdateCartItemRequest(int Quantity);