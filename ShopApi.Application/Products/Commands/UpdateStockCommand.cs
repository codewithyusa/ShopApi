using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Commands;

public record UpdateStockCommand(int ProductId, int Stock)
    : IRequest<Result<ProductResponseDto, ProductError>>;