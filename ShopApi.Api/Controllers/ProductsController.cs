using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Application.Products.Commands;
using ShopApi.Application.Products.Dtos;
using ShopApi.Application.Products.Queries;

namespace ShopApi.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetAllProductsQuery(), ct));

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured(CancellationToken ct) =>
        Ok(await mediator.Send(new GetFeaturedProductsQuery(), ct));

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category, CancellationToken ct) =>
        Ok(await mediator.Send(new GetProductsByCategoryQuery(category), ct));

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateProductCommand(
            request.Name, request.Description, request.Price, request.Image,
            request.Category, request.Color, request.Size, request.Stock, request.IsFeatured), ct);

        return result.Match<IActionResult>(
            onSuccess: p => CreatedAtAction(nameof(GetAll), new { id = p.Id }, p),
            onFailure: error => BadRequest(new ProblemDetails { Status = 400, Title = "Create failed", Detail = error.Message }));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, UpdateStockRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateStockCommand(id, request.Stock), ct);
        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Update failed", Detail = error.Message }));
    }

    [Authorize(Roles = "admin")]
    [HttpPatch("{id}/toggle-featured")]
    public async Task<IActionResult> ToggleFeatured(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ToggleFeaturedCommand(id), ct);
        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Toggle failed", Detail = error.Message }));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteProductCommand(id), ct);
        return result.Match<IActionResult>(
            onSuccess: _ => NoContent(),
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Delete failed", Detail = error.Message }));
    }
}