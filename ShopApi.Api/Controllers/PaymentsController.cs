using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Payments.Commands;
using ShopApi.Application.Payments.Dtos;

namespace ShopApi.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [Authorize]
    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate(InitiatePaymentRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new InitiatePaymentCommand(CurrentUserId, request.OrderId), ct);

        return result.Match<IActionResult>(
            onSuccess: url => Ok(new { checkoutUrl = url }),
            onFailure: error => error.Code switch
            {
                "already_paid" => Conflict(new ProblemDetails { Status = 409, Title = "Payment failed", Detail = error.Message }),
                _ => BadRequest(new ProblemDetails { Status = 400, Title = "Payment failed", Detail = error.Message })
            });
    }

    [AllowAnonymous]
    [HttpGet("verify/{txRef}")]
    public async Task<IActionResult> Verify(string txRef, CancellationToken ct)
    {
        var result = await mediator.Send(new VerifyCheckoutCommand(txRef), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => BadRequest(new ProblemDetails { Status = 400, Title = "Verification failed", Detail = error.Message }));
    }

    [AllowAnonymous]
    [HttpGet("fake-checkout")]
    public async Task<IActionResult> FakeCheckout([FromQuery] string txRef, CancellationToken ct)
    {
        var result = await mediator.Send(new VerifyCheckoutCommand(txRef), ct);

        return result.Match<IActionResult>(
            onSuccess: _ => Redirect("http://localhost:4200/orders"),
            onFailure: error => BadRequest(new ProblemDetails { Status = 400, Title = "Payment failed", Detail = error.Message }));
    }
}