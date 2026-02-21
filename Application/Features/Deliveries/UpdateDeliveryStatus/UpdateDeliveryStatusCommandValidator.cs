using FluentValidation;

namespace Application.Features.Deliveries.UpdateDeliveryStatus;

public class UpdateDeliveryStatusCommandValidator : AbstractValidator<UpdateDeliveryStatusCommand>
{
    public UpdateDeliveryStatusCommandValidator()
    {
        RuleFor(x => x.DeliveryId)
            .NotEmpty().WithMessage("El ID de la entrega es requerido.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Las notas no pueden superar los 500 caracteres.")
            .When(x => x.Notes is not null);
    }
}
