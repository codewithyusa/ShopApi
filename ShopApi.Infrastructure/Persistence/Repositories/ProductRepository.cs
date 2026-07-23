using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Repositories;

public class ProductRepository(ShopDbContext context) : IProductRepository
{
    public Task<Product?> GetByIdAsync(int id, CancellationToken ct) =>
        context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<List<Product>> GetAllAsync(CancellationToken ct) =>
        context.Products.AsNoTracking().ToListAsync(ct);

    public Task<List<Product>> GetFeaturedAsync(CancellationToken ct) =>
        context.Products.AsNoTracking().Where(p => p.IsFeatured).ToListAsync(ct);

    public Task<List<Product>> GetByCategoryAsync(string category, CancellationToken ct) =>
        context.Products.AsNoTracking().Where(p => p.Category == category).ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct) =>
        await context.Products.AddAsync(product, ct);

    public Task DeleteAsync(Product product, CancellationToken ct)
    {
        context.Products.Remove(product);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}