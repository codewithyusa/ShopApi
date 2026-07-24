using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Commands;

public record CreateProductCommand(
    string Name, string? Description, decimal Price, string Image,
    string Category, string Color, string Size, int Stock, bool IsFeatured)
    : IRequest<Result<ProductResponseDto, ProductError>>;