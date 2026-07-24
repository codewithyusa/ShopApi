using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Coupons.Dtos;

namespace ShopApi.Application.Coupons.Commands;

public record ValidateCouponCommand(int UserId, string Code)
    : IRequest<Result<CouponResponseDto, CouponError>>;
    