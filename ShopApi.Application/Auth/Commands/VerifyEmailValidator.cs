using FluentValidation;

namespace ShopApi.Application.Auth.Commands;

public class VerifyEmailValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Code).NotEmpty().Length(6);
    }
}