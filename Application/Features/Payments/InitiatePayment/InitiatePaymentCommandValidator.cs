using FluentValidation;

namespace Application.Features.Payments.InitiatePayment;

public class InitiatePaymentCommandValidator : AbstractValidator<InitiatePaymentCommand>
{
    public InitiatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El ID del pedido es requerido");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("El método de pago no es válido");

        RuleFor(x => x.ReturnUrl)
            .NotEmpty().WithMessage("La URL de retorno es requerida")
            .MaximumLength(500).WithMessage("La URL de retorno no puede superar 500 caracteres");
    }
}
