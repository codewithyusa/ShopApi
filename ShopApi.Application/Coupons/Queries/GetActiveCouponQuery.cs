using MediatR;
using ShopApi.Application.Coupons.Dtos;

namespace ShopApi.Application.Coupons.Queries;

public record GetActiveCouponQuery(int UserId) : IRequest<CouponResponseDto?>;