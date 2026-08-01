using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    (string RawToken, DateTime ExpiresAt) GenerateRefreshToken();
    string HashToken(string rawToken);
}