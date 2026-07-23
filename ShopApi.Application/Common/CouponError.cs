namespace ShopApi.Application.Common;

public sealed record CouponError(string Code, string Message)
{
    public static CouponError NotFound(string code) =>
        new("coupon_not_found", $"Coupon '{code}' was not found.");

    public static CouponError Expired(string code) =>
        new("coupon_expired", $"Coupon '{code}' has expired.");

    public static CouponError Inactive(string code) =>
        new("coupon_inactive", $"Coupon '{code}' is no longer active.");
}