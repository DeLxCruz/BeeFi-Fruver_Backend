using FluentValidation;

namespace Application.Features.Payments.ProcessRefund;

public class ProcessRefundCommandValidator : AbstractValidator<ProcessRefundCommand>
{
    public ProcessRefundCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("El ID del pedido es requerido");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto del reembolso debe ser mayor a cero");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("La razón del reembolso es requerida")
            .MaximumLength(500).WithMessage("La razón no puede superar 500 caracteres");
    }
}
