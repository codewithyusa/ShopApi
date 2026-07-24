using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record UpdateProfileCommand(int UserId, string Name, string? Phone)
    : IRequest<Result<UserResponseDto, AuthError>>;