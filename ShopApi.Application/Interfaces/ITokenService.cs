using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}