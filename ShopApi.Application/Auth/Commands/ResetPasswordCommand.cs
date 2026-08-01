using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record ResetPasswordCommand(string Email, string Code, string NewPassword)
    : IRequest<Result<bool, AuthError>>;