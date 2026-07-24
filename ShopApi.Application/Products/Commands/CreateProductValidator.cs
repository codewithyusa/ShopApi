using FluentValidation;

namespace ShopApi.Application.Products.Commands;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Image).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Size).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}