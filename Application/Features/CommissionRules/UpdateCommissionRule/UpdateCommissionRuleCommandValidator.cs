using FluentValidation;

namespace Application.Features.CommissionRules.UpdateCommissionRule;

public class UpdateCommissionRuleCommandValidator : AbstractValidator<UpdateCommissionRuleCommand>
{
    public UpdateCommissionRuleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID de la regla es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la regla es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.CommissionValue)
            .GreaterThanOrEqualTo(0).WithMessage("El valor de comisión no puede ser negativo.");
    }
}
