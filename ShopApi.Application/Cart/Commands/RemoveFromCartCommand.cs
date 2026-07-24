using MediatR;
using ShopApi.Application.Cart.Dtos;
using ShopApi.Application.Common;

namespace ShopApi.Application.Cart.Commands;

public record RemoveFromCartCommand(int UserId, int ProductId)
    : IRequest<Result<CartResponseDto, CartError>>;