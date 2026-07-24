using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Admin.Commands;

public record DeleteUserCommand(int UserId) : IRequest<Result<bool, AuthError>>;