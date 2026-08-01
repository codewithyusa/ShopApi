using MediatR;
using ShopApi.Application.Analytics.Dtos;

namespace ShopApi.Application.Analytics.Queries;

public record GetTopSellingProductsQuery(int TopN = 10) : IRequest<List<TopSellingProductDto>>;