using MediatR;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Products.Commands;

public class DeleteProductHandler(IProductRepository products)
    : IRequestHandler<DeleteProductCommand, Result<bool, ProductError>>
{
    public async Task<Result<bool, ProductError>> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await products.GetByIdAsync(command.ProductId, ct);
        if (product is null)
            return Result<bool, ProductError>.Failure(ProductError.NotFound(command.ProductId));

        await products.DeleteAsync(product, ct);
        await products.SaveChangesAsync(ct);

        return Result<bool, ProductError>.Success(true);
    }
}