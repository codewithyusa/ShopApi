using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Repositories;

public class UserRepository(ShopDbContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(int id, CancellationToken ct) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        context.Users.AnyAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct) =>
        await context.Users.AddAsync(user, ct);

    public Task<List<User>> GetAllAsync(CancellationToken ct) =>
        context.Users.AsNoTracking().ToListAsync(ct);

    public Task DeleteAsync(User user, CancellationToken ct)
    {
        context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}