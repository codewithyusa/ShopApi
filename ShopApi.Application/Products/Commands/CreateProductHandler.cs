using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Application.Products.Dtos;
using ShopApi.Application.Products.Queries;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Products.Commands;

public class CreateProductHandler(IProductRepository products)
    : IRequestHandler<CreateProductCommand, Result<ProductResponseDto, ProductError>>
{
    public async Task<Result<ProductResponseDto, ProductError>> Handle(
        CreateProductCommand command, CancellationToken ct)
    {
        var product = new Product
        {
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Image = command.Image,
            Category = command.Category,
            Color = command.Color,
            Size = command.Size,
            Stock = command.Stock,
            IsFeatured = command.IsFeatured
        };

        await products.AddAsync(product, ct);
        await products.SaveChangesAsync(ct);

        return Result<ProductResponseDto, ProductError>.Success(GetAllProductsHandler.ToDto(product));
    }
}