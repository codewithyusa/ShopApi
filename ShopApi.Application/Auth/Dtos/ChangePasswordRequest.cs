namespace ShopApi.Application.Auth.Dtos;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);