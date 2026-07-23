using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Auth.Commands;

namespace ShopApi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
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
}