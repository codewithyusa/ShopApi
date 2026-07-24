using FluentValidation;

namespace ShopApi.Application.Auth.Commands;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8)
            .WithMessage("New password must be at least 8 characters.");
    }
}