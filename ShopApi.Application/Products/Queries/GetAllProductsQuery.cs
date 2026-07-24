using MediatR;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public record GetAllProductsQuery : IRequest<List<ProductResponseDto>>;