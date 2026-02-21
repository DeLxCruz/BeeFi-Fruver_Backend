using FluentValidation;

namespace Application.Features.Rewards.CreateReward;

public class CreateRewardCommandValidator : AbstractValidator<CreateRewardCommand>
{
    public CreateRewardCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede superar 200 caracteres.");

        RuleFor(x => x.PointsRequired)
            .GreaterThan(0).WithMessage("Los puntos requeridos deben ser mayores a 0.");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("El valor debe ser mayor a 0.");

        RuleFor(x => x.MaxRedemptionsPerUser)
            .GreaterThanOrEqualTo(1).WithMessage("El máximo de canjes por usuario debe ser al menos 1.");
    }
}
