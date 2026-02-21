using FluentValidation;

namespace Application.Features.Zones.UpdateZone;

public class UpdateZoneCommandValidator : AbstractValidator<UpdateZoneCommand>
{
    public UpdateZoneCommandValidator()
    {
        RuleFor(x => x.ZoneId)
            .NotEmpty().WithMessage("El Id de la zona es requerido");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la zona es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ciudad es requerida")
            .MaximumLength(100).WithMessage("La ciudad no puede superar 100 caracteres");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("El departamento es requerido")
            .MaximumLength(100).WithMessage("El departamento no puede superar 100 caracteres");

        RuleFor(x => x.DeliveryBaseCost)
            .GreaterThanOrEqualTo(0).WithMessage("El costo base de entrega no puede ser negativo");
    }
}
