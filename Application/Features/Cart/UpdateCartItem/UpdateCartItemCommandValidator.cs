using FluentValidation;

namespace Application.Features.Cart.UpdateCartItem;

public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.CartItemId)
            .NotEmpty().WithMessage("El item del carrito es requerido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero")
            .LessThanOrEqualTo(100).WithMessage("La cantidad máxima por item es 100");
    }
}
