using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Api.Auth;
using ShopApi.Application.Auth.Commands;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Auth.Queries;

namespace ShopApi.Api.Controllers;

[EnableRateLimiting("auth")]
[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator, IWebHostEnvironment env) : ControllerBase
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
            onSuccess: dto =>
            {
                SetRefreshCookie(dto.RefreshToken!, dto.RefreshTokenExpiresAt!.Value);
                return Ok(dto);
            },
            onFailure: error => error.Code switch
            {
                "email_not_verified" => Unauthorized(new ProblemDetails
                {
                    Status = 401,
                    Title = "Email not verified",
                    Detail = error.Message
                }),
                _ => Unauthorized(new ProblemDetails
                {
                    Status = 401,
                    Title = "Login failed",
                    Detail = error.Message
                })
            });
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

    // NOTE: no [Authorize] here — an unverified user has no JWT yet, so this
    // must be reachable using just their email instead of CurrentUserId.
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new VerifyEmailByEmailCommand(request.Email, request.Code), ct);

        return result.Match<IActionResult>(
            onSuccess: _ => Ok(new { message = "Email verified." }),
            onFailure: error => BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Verification failed",
                Detail = error.Message
            }));
    }

    // NOTE: also no [Authorize] — same reason. Useful if the code expires
    // and the user needs a fresh one before they can verify/login.
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(ResendVerificationRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new SendVerificationEmailByEmailCommand(request.Email), ct);

        return result.Match<IActionResult>(
            onSuccess: _ => Ok(new { message = "Verification code sent." }),
            onFailure: error => BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Send failed",
                Detail = error.Message
            }));
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken ct)
    {
        await mediator.Send(new ForgotPasswordCommand(request.Email), ct);
        return Ok(new { message = "If that email is registered, a reset code has been sent." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(
            new ResetPasswordCommand(request.Email, request.Code, request.NewPassword), ct);

        return result.Match<IActionResult>(
            onSuccess: _ => Ok(new { message = "Password has been reset. Please log in again." }),
            onFailure: error => BadRequest(new ProblemDetails
            {
                Status = 400,
                Title = "Reset failed",
                Detail = error.Message
            }));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var rawToken = Request.Cookies[CookieAuthOptions.RefreshTokenCookieName];
        if (string.IsNullOrWhiteSpace(rawToken))
            return Unauthorized(new ProblemDetails
            {
                Status = 401,
                Title = "Refresh failed",
                Detail = "No refresh token present."
            });

        var result = await mediator.Send(new RefreshTokenCommand(rawToken), ct);

        return result.Match<IActionResult>(
            onSuccess: dto =>
            {
                SetRefreshCookie(dto.RefreshToken!, dto.RefreshTokenExpiresAt!.Value);
                return Ok(dto);
            },
            onFailure: error =>
            {
                Response.Cookies.Delete(CookieAuthOptions.RefreshTokenCookieName);
                return Unauthorized(new ProblemDetails
                {
                    Status = 401,
                    Title = "Refresh failed",
                    Detail = error.Message
                });
            });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var rawToken = Request.Cookies[CookieAuthOptions.RefreshTokenCookieName];
        await mediator.Send(new LogoutCommand(rawToken ?? string.Empty), ct);
        Response.Cookies.Delete(CookieAuthOptions.RefreshTokenCookieName);
        return NoContent();
    }

    private void SetRefreshCookie(string rawToken, DateTime expiresAt) =>
        Response.Cookies.Append(
            CookieAuthOptions.RefreshTokenCookieName, rawToken,
            CookieAuthOptions.Build(expiresAt, env));
}

// Outside the class
public record VerifyEmailRequest(string Email, string Code);
public record ResendVerificationRequest(string Email);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Email, string Code, string NewPassword);