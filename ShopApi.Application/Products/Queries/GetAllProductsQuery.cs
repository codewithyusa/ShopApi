using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Products.Dtos;

namespace ShopApi.Application.Products.Queries;

public record GetAllProductsQuery(PagedRequest Paging) : IRequest<PagedResponse<ProductResponseDto>>;