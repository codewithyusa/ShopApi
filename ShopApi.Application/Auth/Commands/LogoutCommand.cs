using MediatR;

namespace ShopApi.Application.Auth.Commands;

public record LogoutCommand(string RawRefreshToken) : IRequest<bool>;