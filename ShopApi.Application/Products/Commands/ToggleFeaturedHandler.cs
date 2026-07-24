using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;
using ShopApi.Application.Products.Queries;

namespace ShopApi.Application.Products.Commands;

public class ToggleFeaturedHandler(IProductRepository products)
    : IRequestHandler<ToggleFeaturedCommand, Result<ProductResponseDto, ProductError>>
{
    public async Task<Result<ProductResponseDto, ProductError>> Handle(
        ToggleFeaturedCommand command, CancellationToken ct)
    {
        var product = await products.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result<ProductResponseDto, ProductError>.Failure(ProductError.NotFound(command.ProductId));

        product.IsFeatured = !product.IsFeatured;
        product.UpdatedAt = DateTime.UtcNow;
        await products.SaveChangesAsync(ct);

        return Result<ProductResponseDto, ProductError>.Success(GetAllProductsHandler.ToDto(product));
    }
}