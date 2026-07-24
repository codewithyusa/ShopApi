using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Commands;

public record ToggleFeaturedCommand(int ProductId)
    : IRequest<Result<ProductResponseDto, ProductError>>;