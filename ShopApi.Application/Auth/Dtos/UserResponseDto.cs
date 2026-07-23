namespace ShopApi.Application.Auth.Dtos;

public record UserResponseDto(int Id, string Name, string Email, string? Phone, string Role);