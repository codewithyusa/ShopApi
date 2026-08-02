using ShopApi.Application.Common;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken ct);
    Task<List<Order>> GetAllAsync(CancellationToken ct);
    Task<(List<Order> Items, int TotalCount)> GetAllPagedAsync(PagedRequest request, CancellationToken ct);
    Task<Order?> GetByPaymentRefAsync(string paymentRef, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}