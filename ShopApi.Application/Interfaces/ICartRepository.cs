using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface ICartRepository
{
    Task<List<CartItem>> GetByUserIdAsync(int userId, CancellationToken ct);
    Task<CartItem?> GetItemAsync(int userId, int productId, CancellationToken ct);
    Task AddAsync(CartItem item, CancellationToken ct);
    Task RemoveAsync(CartItem item, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}