using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken ct);
    Task<List<Order>> GetAllAsync(CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}