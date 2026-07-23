using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record SignupCommand(string Name, string Email, string Password, string? Phone)
    : IRequest<Result<UserResponseDto, AuthError>>;