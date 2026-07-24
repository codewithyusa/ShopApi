using FluentValidation;

namespace ShopApi.Application.Cart.Commands;

public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}