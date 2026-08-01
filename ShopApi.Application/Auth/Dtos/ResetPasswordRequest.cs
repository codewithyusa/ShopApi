namespace ShopApi.Application.Auth.Dtos;

public record ResetPasswordRequest(string Email, string Code, string NewPassword);