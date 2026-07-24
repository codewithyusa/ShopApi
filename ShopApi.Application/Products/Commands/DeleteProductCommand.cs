using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Products.Commands;

public record DeleteProductCommand(int ProductId) : IRequest<Result<bool, ProductError>>;