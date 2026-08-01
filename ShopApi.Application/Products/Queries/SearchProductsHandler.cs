using MediatR;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public class SearchProductsHandler(IProductRepository products)
    : IRequestHandler<SearchProductsQuery, List<ProductResponseDto>>
{
    public async Task<List<ProductResponseDto>> Handle(SearchProductsQuery query, CancellationToken ct)
    {
        var results = await products.SearchAsync(
            query.Name, query.Category, query.MinPrice, query.MaxPrice, query.InStockOnly, ct);

        return results.Select(GetAllProductsHandler.ToDto).ToList();
    }
}