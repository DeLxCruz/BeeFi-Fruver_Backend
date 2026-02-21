using FluentValidation;

namespace Application.Features.Cart.AddToCart;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.FruverProductId)
            .NotEmpty().WithMessage("El producto es requerido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero")
            .LessThanOrEqualTo(100).WithMessage("La cantidad máxima por item es 100");
    }
}
