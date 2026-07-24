using MediatR;
using ShopApi.Application.Coupons.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Coupons.Queries;

public class GetActiveCouponHandler(ICouponRepository coupons)
    : IRequestHandler<GetActiveCouponQuery, CouponResponseDto?>
{
    public async Task<CouponResponseDto?> Handle(GetActiveCouponQuery query, CancellationToken ct)
    {
        var coupon = await coupons.GetActiveForUserAsync(query.UserId, ct);
        return coupon is null
            ? null
            : new CouponResponseDto(coupon.Id, coupon.Code, coupon.DiscountPercentage, coupon.ExpirationDate);
    }
}