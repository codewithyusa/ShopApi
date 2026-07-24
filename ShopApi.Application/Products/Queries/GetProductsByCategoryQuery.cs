using MediatR;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public record GetProductsByCategoryQuery(string Category) : IRequest<List<ProductResponseDto>>;