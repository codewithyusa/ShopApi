using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public class SearchProductsHandler(IProductRepository products)
    : IRequestHandler<SearchProductsQuery, PagedResponse<ProductResponseDto>>
{
    public async Task<PagedResponse<ProductResponseDto>> Handle(SearchProductsQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await products.SearchPagedAsync(
            query.Name, query.Category, query.MinPrice, query.MaxPrice, query.InStockOnly, query.Paging, ct);

        return new PagedResponse<ProductResponseDto>
        {
            Items = items.Select(GetAllProductsHandler.ToDto).ToList(),
            TotalCount = totalCount,
            Page = query.Paging.Page,
            PageSize = query.Paging.PageSize
        };
    }
}