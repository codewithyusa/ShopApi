using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record SendVerificationEmailByEmailCommand(string Email)
    : IRequest<Result<bool, EmailVerificationError>>;