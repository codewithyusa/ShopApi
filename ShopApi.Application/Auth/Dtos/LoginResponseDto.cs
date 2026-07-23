namespace ShopApi.Application.Auth.Dtos;

public record LoginResponseDto(string Token, UserResponseDto User);