using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Coupons.Commands;
using ShopApi.Application.Coupons.Queries;

namespace ShopApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/coupons")]
public class CouponsController(IMediator mediator) : ControllerBase
{
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet("active")]
    public async Task<IActionResult> GetActive(CancellationToken ct)
    {
        var coupon = await mediator.Send(new GetActiveCouponQuery(CurrentUserId), ct);
        return coupon is not null ? Ok(coupon) : NotFound();
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate(ValidateCouponRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new ValidateCouponCommand(CurrentUserId, request.Code), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => BadRequest(new ProblemDetails { Status = 400, Title = "Invalid coupon", Detail = error.Message }));
    }
}

public record ValidateCouponRequest(string Code);