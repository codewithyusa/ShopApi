using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Coupons.Dtos;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Coupons.Commands;

public class ValidateCouponHandler(ICouponRepository coupons)
    : IRequestHandler<ValidateCouponCommand, Result<CouponResponseDto, CouponError>>
{
    public async Task<Result<CouponResponseDto, CouponError>> Handle(
        ValidateCouponCommand command, CancellationToken ct)
    {
        var coupon = await coupons.GetByCodeAsync(command.Code, ct);
        if (coupon is null)
            return Result<CouponResponseDto, CouponError>.Failure(CouponError.NotFound(command.Code));

        if (!coupon.IsActive)
            return Result<CouponResponseDto, CouponError>.Failure(CouponError.Inactive(command.Code));

        if (coupon.ExpirationDate < DateTime.UtcNow)
            return Result<CouponResponseDto, CouponError>.Failure(CouponError.Expired(command.Code));

        return Result<CouponResponseDto, CouponError>.Success(
            new CouponResponseDto(coupon.Id, coupon.Code, coupon.DiscountPercentage, coupon.ExpirationDate));
    }
}