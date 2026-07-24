using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;

namespace ShopApi.Application.Admin.Commands;

public record UpdateUserRoleCommand(int UserId, string Role)
    : IRequest<Result<UserResponseDto, AuthError>>;