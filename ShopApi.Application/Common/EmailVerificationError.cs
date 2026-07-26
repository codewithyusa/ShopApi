namespace ShopApi.Application.Common;

public sealed record EmailVerificationError(string Code, string Message)
{
    public static EmailVerificationError UserNotFound() =>
        new("user_not_found", "User was not found.");

    public static EmailVerificationError AlreadyVerified() =>
        new("already_verified", "Email is already verified.");

    public static EmailVerificationError InvalidCode() =>
        new("invalid_code", "Verification code is invalid or expired.");
}