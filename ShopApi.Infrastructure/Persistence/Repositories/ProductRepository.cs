using Microsoft.EntityFrameworkCore;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;
using ShopApi.Domain.Entities;

namespace ShopApi.Infrastructure.Persistence.Repositories;

public class ProductRepository(ShopDbContext context) : IProductRepository
{
    public Task<Product?> GetByIdAsync(int id, CancellationToken ct) =>
        context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<List<Product>> GetAllAsync(CancellationToken ct) =>
        context.Products.AsNoTracking().ToListAsync(ct);

    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        PagedRequest request, CancellationToken ct)
    {
        var query = context.Products.AsNoTracking().OrderBy(p => p.Name);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public Task<List<Product>> GetFeaturedAsync(CancellationToken ct) =>
        context.Products.AsNoTracking().Where(p => p.IsFeatured).ToListAsync(ct);

    public Task<List<Product>> GetByCategoryAsync(string category, CancellationToken ct) =>
        context.Products.AsNoTracking().Where(p => p.Category == category).ToListAsync(ct);

    public Task<List<Product>> SearchAsync(
        string? name, string? category, decimal? minPrice, decimal? maxPrice, bool? inStockOnly,
        CancellationToken ct)
    {
        var query = context.Products.AsNoTracking().AsQueryable();

        // ILike = case-insensitive LIKE on PostgreSQL; matches "shirt" against "T-Shirt".
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{name}%"));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (inStockOnly == true)
            query = query.Where(p => p.Stock > 0);

        return query.ToListAsync(ct);
    }

    public async Task<(List<Product> Items, int TotalCount)> SearchPagedAsync(
        string? name, string? category, decimal? minPrice, decimal? maxPrice, bool? inStockOnly,
        PagedRequest request, CancellationToken ct)
    {
        var query = context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{name}%"));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (inStockOnly == true)
            query = query.Where(p => p.Stock > 0);

        query = query.OrderBy(p => p.Name);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

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