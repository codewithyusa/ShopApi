using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface IFavoriteRepository
{
    Task<List<Favorite>> GetByUserIdAsync(int userId, CancellationToken ct);
    Task<Favorite?> GetAsync(int userId, int productId, CancellationToken ct);
    Task AddAsync(Favorite favorite, CancellationToken ct);
    Task RemoveAsync(Favorite favorite, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}