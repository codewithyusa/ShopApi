using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task<List<User>> GetAllAsync(CancellationToken ct);
    Task DeleteAsync(User user, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}