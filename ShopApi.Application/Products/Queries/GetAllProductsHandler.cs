using MediatR;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public class GetAllProductsHandler(IProductRepository products)
    : IRequestHandler<GetAllProductsQuery, List<ProductResponseDto>>
{
    public async Task<List<ProductResponseDto>> Handle(GetAllProductsQuery query, CancellationToken ct)
    {
        var list = await products.GetAllAsync(ct);
        return list.Select(ToDto).ToList();
    }

    public static ProductResponseDto ToDto(Domain.Entities.Product p) =>
        new(p.Id, p.Name, p.Description, p.Price, p.Image, p.Category, p.Color, p.Size, p.Stock, p.IsFeatured);
}