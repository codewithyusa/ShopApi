using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;
using ShopApi.Application.Products.Queries;

namespace ShopApi.Application.Products.Commands;

public class UpdateProductHandler(IProductRepository products)
    : IRequestHandler<UpdateProductCommand, Result<ProductResponseDto, ProductError>>
{
    public async Task<Result<ProductResponseDto, ProductError>> Handle(
        UpdateProductCommand command, CancellationToken ct)
    {
        var product = await products.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result<ProductResponseDto, ProductError>.Failure(ProductError.NotFound(command.ProductId));

        var req = command.Request;

        if (req.Name is not null) product.Name = req.Name;
        if (req.Description is not null) product.Description = req.Description;
        if (req.Price is not null) product.Price = req.Price.Value;
        if (req.Image is not null) product.Image = req.Image;
        if (req.Category is not null) product.Category = req.Category;
        if (req.Color is not null) product.Color = req.Color;
        if (req.Size is not null) product.Size = req.Size;
        if (req.Stock is not null) product.Stock = req.Stock.Value;
        if (req.IsFeatured is not null) product.IsFeatured = req.IsFeatured.Value;

        product.UpdatedAt = DateTime.UtcNow;
        await products.SaveChangesAsync(ct);

        return Result<ProductResponseDto, ProductError>.Success(GetAllProductsHandler.ToDto(product));
    }
}
