using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record RefreshTokenCommand(string RawRefreshToken) : IRequest<Result<LoginResponseDto, AuthError>>;