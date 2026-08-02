using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public record SearchProductsQuery(
    string? Name,
    string? Category,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStockOnly,
    PagedRequest Paging) : IRequest<PagedResponse<ProductResponseDto>>;