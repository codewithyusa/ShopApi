using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record LoginCommand(string Email, string Password)
    : IRequest<Result<LoginResponseDto, AuthError>>;