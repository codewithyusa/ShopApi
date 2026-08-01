using System.Text.Json.Serialization;

namespace ShopApi.Application.Auth.Dtos;

public record LoginResponseDto(string Token, UserResponseDto User)
{
    // Never leaves the server as JSON — the controller reads these to set the cookie,
    // then the client only ever sees Token + User in the response body.
    [JsonIgnore] public string? RefreshToken { get; init; }
    [JsonIgnore] public DateTime? RefreshTokenExpiresAt { get; init; }
}