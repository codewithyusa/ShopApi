namespace ShopApi.Application.Auth.Dtos;

public record SignupRequest(string Name, string Email, string Password, string? Phone);