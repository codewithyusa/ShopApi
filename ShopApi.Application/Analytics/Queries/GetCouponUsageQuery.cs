using MediatR;
using ShopApi.Application.Analytics.Dtos;

namespace ShopApi.Application.Analytics.Queries;

public record GetCouponUsageQuery : IRequest<List<CouponUsageDto>>;