using MediatR;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public class GetFeaturedProductsHandler(IProductRepository products)
    : IRequestHandler<GetFeaturedProductsQuery, List<ProductResponseDto>>
{
    public async Task<List<ProductResponseDto>> Handle(GetFeaturedProductsQuery query, CancellationToken ct)
    {
        var list = await products.GetFeaturedAsync(ct);
        return list.Select(GetAllProductsHandler.ToDto).ToList();
    }
}