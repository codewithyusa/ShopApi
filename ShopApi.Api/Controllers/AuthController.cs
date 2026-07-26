using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Auth.Commands;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Auth.Queries;

namespace ShopApi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    // Your existing actions:
    // Signup
    // Login
    // GetProfile
    // UpdateProfile
    // ChangePassword


    [Authorize]
    [HttpPost("send-verification")]
    public async Task<IActionResult> SendVerification(CancellationToken ct)
    {
        var result = await mediator.Send(
            new SendVerificationEmailCommand(CurrentUserId), ct);

        return result.Match<IActionResult>(
            onSuccess: _ => Ok(new
            {
                message = "Verification code sent."
            }),
            onFailure: error => BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Send failed",
                Detail = error.Message
            }));
    }


    [Authorize]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        VerifyEmailRequest request,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new VerifyEmailCommand(CurrentUserId, request.Code), ct);

        return result.Match<IActionResult>(
            onSuccess: _ => Ok(new
            {
                message = "Email verified."
            }),
            onFailure: error => BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Verification failed",
                Detail = error.Message
            }));
    }
}


// Outside the class
public record VerifyEmailRequest(string Code);