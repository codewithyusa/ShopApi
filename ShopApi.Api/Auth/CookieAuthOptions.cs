namespace ShopApi.Api.Auth;

public static class CookieAuthOptions
{
    public const string RefreshTokenCookieName = "shop_refresh_token";

    public static CookieOptions Build(DateTime expiresAt, IWebHostEnvironment env) => new()
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(), // allow http on localhost, require https elsewhere
        SameSite = SameSiteMode.Strict,
        Expires = expiresAt,
        Path = "/api/auth" // never sent to non-auth endpoints
    };
}