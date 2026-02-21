using FluentValidation;

namespace Application.Features.Zones.AssignFruverToZone;

public class AssignFruverToZoneCommandValidator : AbstractValidator<AssignFruverToZoneCommand>
{
    public AssignFruverToZoneCommandValidator()
    {
        RuleFor(x => x.FruverId)
            .NotEmpty().WithMessage("El Id del fruver es requerido");

        RuleFor(x => x.ZoneId)
            .NotEmpty().WithMessage("El Id de la zona es requerido");
    }
}
