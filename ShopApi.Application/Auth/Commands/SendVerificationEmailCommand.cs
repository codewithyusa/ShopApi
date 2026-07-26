using MediatR;
using ShopApi.Application.Common;

namespace ShopApi.Application.Auth.Commands;

public record SendVerificationEmailCommand(int UserId) : IRequest<Result<bool, EmailVerificationError>>;