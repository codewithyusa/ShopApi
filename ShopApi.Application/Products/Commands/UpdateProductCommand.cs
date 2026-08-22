using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Commands;

public record UpdateProductCommand(int ProductId, UpdateProductRequest Request)
    : IRequest<Result<ProductResponseDto, ProductError>>;
