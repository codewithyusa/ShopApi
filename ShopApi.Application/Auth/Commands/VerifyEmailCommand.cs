using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record VerifyEmailCommand(int UserId, string Code) : IRequest<Result<bool, EmailVerificationError>>;