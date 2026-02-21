using FluentValidation;

namespace Application.Features.Orders.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.AddressId)
            .NotEmpty().WithMessage("La dirección de entrega es requerida");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("El método de pago no es válido");
    }
}
