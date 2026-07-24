using MediatR;
using ShopApi.Application.Auth.Dtos;

namespace ShopApi.Application.Auth.Queries;

public record GetProfileQuery(int UserId) : IRequest<UserResponseDto?>;