using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public record GetProductByIdQuery(int Id) : IRequest<Result<ProductResponseDto, ProductError>>;