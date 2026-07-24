using MediatR;
using ShopApi.Application.Auth.Dtos;

namespace ShopApi.Application.Admin.Queries;

public record GetUserByIdQuery(int UserId) : IRequest<UserResponseDto?>;