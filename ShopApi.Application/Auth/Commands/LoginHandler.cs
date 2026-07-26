using MediatR;
using ShopApi.Application.Auth.Dtos;
using ShopApi.Application.Common;
using ShopApi.Application.Interfaces;

namespace ShopApi.Application.Auth.Commands;

public class LoginHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    ITokenService tokens,
    IMediator mediator)
    : IRequestHandler<LoginCommand, Result<LoginResponseDto, AuthError>>
{
    public async Task<Result<LoginResponseDto, AuthError>> Handle(
        LoginCommand command, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(command.Email, ct);
        if (user is null || !hasher.Verify(command.Password, user.PasswordHash))
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.InvalidCredentials());

        if (!user.IsEmailVerified)
        {
            // Auto-resend a fresh OTP so the customer isn't stuck without one.
            await mediator.Send(new SendVerificationEmailCommand(user.Id), ct);
            return Result<LoginResponseDto, AuthError>.Failure(AuthError.EmailNotVerified());
        }

        var token = tokens.GenerateToken(user);
        var dto = new UserResponseDto(user.Id, user.Name, user.Email, user.Phone, user.Role);

        return Result<LoginResponseDto, AuthError>.Success(new LoginResponseDto(token, dto));
    }
}