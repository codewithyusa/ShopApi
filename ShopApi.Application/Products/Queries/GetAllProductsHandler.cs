using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public class GetAllProductsHandler(IProductRepository products)
    : IRequestHandler<GetAllProductsQuery, PagedResponse<ProductResponseDto>>
{
    public async Task<PagedResponse<ProductResponseDto>> Handle(GetAllProductsQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await products.GetPagedAsync(query.Paging, ct);

        return new PagedResponse<ProductResponseDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = query.Paging.Page,
            PageSize = query.Paging.PageSize
        };
    }

    public static ProductResponseDto ToDto(Domain.Entities.Product p) =>
        new(p.Id, p.Name, p.Description, p.Price, p.Image, p.Category, p.Color, p.Size, p.Stock, p.IsFeatured);
}