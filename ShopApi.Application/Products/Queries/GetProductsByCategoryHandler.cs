using MediatR;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public class GetProductsByCategoryHandler(IProductRepository products)
    : IRequestHandler<GetProductsByCategoryQuery, List<ProductResponseDto>>
{
    public async Task<List<ProductResponseDto>> Handle(GetProductsByCategoryQuery query, CancellationToken ct)
    {
        var list = await products.GetByCategoryAsync(query.Category, ct);
        return list.Select(GetAllProductsHandler.ToDto).ToList();
    }
}