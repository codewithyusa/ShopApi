using MediatR;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public record GetFeaturedProductsQuery : IRequest<List<ProductResponseDto>>;