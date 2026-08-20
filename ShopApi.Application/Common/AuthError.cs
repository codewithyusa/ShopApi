namespace ShopApi.Application.Common;
public sealed record AuthError(string Code, string Message)
{
    public static AuthError EmailAlreadyExists(string email) =>
        new("email_exists", $"An account with email '{email}' already exists.");
    public static AuthError InvalidCredentials() =>
        new("invalid_credentials", "Email or password is incorrect.");
    public static AuthError InvalidCredentials(string detail) =>
        new("invalid_credentials", detail);
    public static AuthError UserNotFound() =>
        new("user_not_found", "User was not found.");
    public static AuthError InvalidResetToken() =>
        new("invalid_reset_token", "Password reset token is invalid or expired.");
    public static AuthError EmailNotVerified() =>
        new("email_not_verified", "Please verify your email before logging in. A new code has been sent.");
    public static AuthError InvalidRefreshToken() =>
        new("invalid_refresh_token", "Refresh token is invalid, expired, or has been revoked.");
    public static AuthError AccountLocked() =>
        new("account_locked", "Account locked due to multiple failed login attempts. Try again in 15 minutes.");
}