using FluentValidation;

namespace Application.Features.Deliveries.AssignDeliveryPerson;

public class AssignDeliveryPersonCommandValidator : AbstractValidator<AssignDeliveryPersonCommand>
{
    public AssignDeliveryPersonCommandValidator()
    {
        RuleFor(x => x.DeliveryId)
            .NotEmpty().WithMessage("El ID de la entrega es requerido.");

        RuleFor(x => x.DeliveryPersonId)
            .NotEmpty().WithMessage("El ID del repartidor es requerido.");
    }
}
