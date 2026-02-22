using FluentValidation;

namespace Application.Features.CommissionRules.CreateCommissionRule;

public class CreateCommissionRuleCommandValidator : AbstractValidator<CreateCommissionRuleCommand>
{
    public CreateCommissionRuleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la regla es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.CommissionValue)
            .GreaterThanOrEqualTo(0).WithMessage("El valor de comisión no puede ser negativo.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage("La prioridad debe ser mayor o igual a 0.");
    }
}
