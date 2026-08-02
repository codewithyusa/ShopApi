using ShopApi.Application.Common;
using ShopApi.Domain.Entities;

namespace ShopApi.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<Product>> GetAllAsync(CancellationToken ct);
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(PagedRequest request, CancellationToken ct);
    Task<List<Product>> GetFeaturedAsync(CancellationToken ct);
    Task<List<Product>> GetByCategoryAsync(string category, CancellationToken ct);
    Task<List<Product>> SearchAsync(
        string? name, string? category, decimal? minPrice, decimal? maxPrice, bool? inStockOnly,
        CancellationToken ct);
    Task<(List<Product> Items, int TotalCount)> SearchPagedAsync(
        string? name, string? category, decimal? minPrice, decimal? maxPrice, bool? inStockOnly,
        PagedRequest request, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task DeleteAsync(Product product, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}