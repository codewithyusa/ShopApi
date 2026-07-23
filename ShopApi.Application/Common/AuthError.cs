namespace ShopApi.Application.Common;

public sealed record AuthError(string Code, string Message)
{
    public static AuthError EmailAlreadyExists(string email) =>
        new("email_exists", $"An account with email '{email}' already exists.");

    public static AuthError InvalidCredentials() =>
        new("invalid_credentials", "Email or password is incorrect.");

    public static AuthError UserNotFound() =>
        new("user_not_found", "User was not found.");

    public static AuthError InvalidResetToken() =>
        new("invalid_reset_token", "Password reset token is invalid or expired.");
}