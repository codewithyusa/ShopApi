using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Orders.Commands;
using ShopApi.Application.Orders.Queries;

namespace ShopApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest? request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateOrderCommand(CurrentUserId, request?.CouponCode), ct);

        return result.Match<IActionResult>(
            onSuccess: order => CreatedAtAction(nameof(GetMyOrders), null, order),
            onFailure: error => error.Code switch
            {
                "cart_empty" => BadRequest(new ProblemDetails { Status = 400, Title = "Checkout failed", Detail = error.Message }),
                _ => Conflict(new ProblemDetails { Status = 409, Title = "Checkout failed", Detail = error.Message })
            });
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders(CancellationToken ct) =>
        Ok(await mediator.Send(new GetUserOrdersQuery(CurrentUserId), ct));

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new CancelOrderCommand(CurrentUserId, id), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => error.Code switch
            {
                "order_not_found" => NotFound(new ProblemDetails { Status = 404, Title = "Cancel failed", Detail = error.Message }),
                _ => Conflict(new ProblemDetails { Status = 409, Title = "Cancel failed", Detail = error.Message })
            });
    }

    [Authorize(Roles = "admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrders(CancellationToken ct) =>
        Ok(await mediator.Send(new GetAllOrdersQuery(), ct));
}

public record CheckoutRequest(string? CouponCode);