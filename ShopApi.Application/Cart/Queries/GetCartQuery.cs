using MediatR;
using ShopApi.Application.Cart.Dtos;

namespace ShopApi.Application.Cart.Queries;

public record GetCartQuery(int UserId) : IRequest<CartResponseDto>;