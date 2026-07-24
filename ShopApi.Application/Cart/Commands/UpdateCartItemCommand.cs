using MediatR;
using ShopApi.Application.Cart.Dtos;
using ShopApi.Application.Common;

namespace ShopApi.Application.Cart.Commands;

public record UpdateCartItemCommand(int UserId, int ProductId, int Quantity)
    : IRequest<Result<CartResponseDto, CartError>>;