using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record VerifyEmailByEmailCommand(string Email, string Code)
    : IRequest<Result<bool, EmailVerificationError>>;
    