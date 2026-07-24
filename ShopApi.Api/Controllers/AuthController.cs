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

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(SignupCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.Match<IActionResult>(
            onSuccess: user => CreatedAtAction(nameof(Signup), new { id = user.Id }, user),
            onFailure: error => error.Code switch
            {
                "email_exists" => Conflict(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Signup failed",
                    Detail = error.Message
                }),
                _ => BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Signup failed",
                    Detail = error.Message
                })
            });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Login failed",
                Detail = error.Message
            }));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var profile = await mediator.Send(new GetProfileQuery(CurrentUserId), ct);
        return profile is not null ? Ok(profile) : NotFound();
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new UpdateProfileCommand(CurrentUserId, request.Name, request.Phone), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Update failed",
                Detail = error.Message
            }));
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new ChangePasswordCommand(CurrentUserId, request.CurrentPassword, request.NewPassword), ct);

        return result.Match<IActionResult>(
            onSuccess: _ => NoContent(),
            onFailure: error => Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Change password failed",
                Detail = error.Message
            }));
    }
}