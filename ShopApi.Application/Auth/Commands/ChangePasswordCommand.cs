using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword)
    : IRequest<Result<bool, AuthError>>;