using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public class GetProductByIdHandler(IProductRepository products)
    : IRequestHandler<GetProductByIdQuery, Result<ProductResponseDto, ProductError>>
{
    public async Task<Result<ProductResponseDto, ProductError>> Handle(
        GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await products.GetByIdAsync(query.Id, ct);

        if (product is null)
            return Result<ProductResponseDto, ProductError>.Failure(
                ProductError.NotFound(query.Id));

        return Result<ProductResponseDto, ProductError>.Success(
            GetAllProductsHandler.ToDto(product));
    }
}