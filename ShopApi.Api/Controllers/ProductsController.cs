using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using ShopApi.Application.Common;
using ShopApi.Application.Products.Commands;
using ShopApi.Application.Products.Dtos;
using ShopApi.Application.Products.Queries;

namespace ShopApi.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IMediator mediator, IOutputCacheStore outputCache) : ControllerBase
{
    [HttpGet]
    [OutputCache(PolicyName = "Products")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetAllProductsQuery(new PagedRequest { Page = page, PageSize = pageSize }), ct));

    [HttpGet("featured")]
    [OutputCache(PolicyName = "Products")]
    public async Task<IActionResult> GetFeatured(CancellationToken ct) =>
        Ok(await mediator.Send(new GetFeaturedProductsQuery(), ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Not found", Detail = error.Message }));
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category, CancellationToken ct) =>
        Ok(await mediator.Send(new GetProductsByCategoryQuery(category), ct));

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStockOnly,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default) =>
        Ok(await mediator.Send(new SearchProductsQuery(
            name, category, minPrice, maxPrice, inStockOnly,
            new PagedRequest { Page = page, PageSize = pageSize }), ct));

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateProductCommand(
            request.Name, request.Description, request.Price, request.Image,
            request.Category, request.Color, request.Size, request.Stock, request.IsFeatured), ct);

        if (result.IsSuccess)
            await outputCache.EvictByTagAsync("products", ct);

        return result.Match<IActionResult>(
            onSuccess: p => CreatedAtAction(nameof(GetAll), new { id = p.Id }, p),
            onFailure: error => BadRequest(new ProblemDetails { Status = 400, Title = "Create failed", Detail = error.Message }));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, UpdateStockRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateStockCommand(id, request.Stock), ct);

        if (result.IsSuccess)
            await outputCache.EvictByTagAsync("products", ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Update failed", Detail = error.Message }));
    }

    [Authorize(Roles = "admin")]
    [HttpPatch("{id}/toggle-featured")]
    public async Task<IActionResult> ToggleFeatured(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new ToggleFeaturedCommand(id), ct);

        if (result.IsSuccess)
            await outputCache.EvictByTagAsync("products", ct);

        return result.Match<IActionResult>(
            onSuccess: Ok,
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Toggle failed", Detail = error.Message }));
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteProductCommand(id), ct);

        if (result.IsSuccess)
            await outputCache.EvictByTagAsync("products", ct);

        return result.Match<IActionResult>(
            onSuccess: _ => NoContent(),
            onFailure: error => NotFound(new ProblemDetails { Status = 404, Title = "Delete failed", Detail = error.Message }));
    }
}