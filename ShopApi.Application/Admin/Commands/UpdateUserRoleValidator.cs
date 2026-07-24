using FluentValidation;

namespace ShopApi.Application.Admin.Commands;

public class UpdateUserRoleValidator : AbstractValidator<UpdateUserRoleCommand>
{
    private static readonly string[] AllowedRoles = ["customer", "admin"];

    public UpdateUserRoleValidator()
    {
        RuleFor(x => x.Role)
            .Must(r => AllowedRoles.Contains(r))
            .WithMessage($"Role must be one of: {string.Join(", ", AllowedRoles)}.");
    }
}