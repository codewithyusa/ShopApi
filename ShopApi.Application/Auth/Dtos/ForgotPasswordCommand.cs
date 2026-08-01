using MediatR;

namespace ShopApi.Application.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<bool>;