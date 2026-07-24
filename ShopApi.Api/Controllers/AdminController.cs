using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Admin.Commands;
using ShopApi.Application.Admin.Dtos;
using ShopApi.Application.Admin.Queries;

namespace ShopApi.Api.Controllers;

[Authorize(Roles = "admin")]
[ApiController]
[Route("api/admin")]
public class AdminController(IMediator mediator) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct) =>
        Ok(await mediator.Send(new GetAllUsersQuery(), ct));

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(int id, CancellationToken ct)
    {
        var user = await mediator.Send(new GetUserByIdQuery(id), ct);
        return user is not null ? Ok(user) : NotFound();
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, UpdateUserRoleRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateUserRoleCommand(id, request.Role), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Update failed", Detail = error.Message }));
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteUserCommand(id), ct);

        return result.Match<IActionResult>(
            onSuccess: _ => NoContent(),
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Delete failed", Detail = error.Message }));
    }
}