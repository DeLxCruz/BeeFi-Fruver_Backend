using FluentValidation;

namespace Application.Features.Rewards.RedeemReward;

public class RedeemRewardCommandValidator : AbstractValidator<RedeemRewardCommand>
{
    public RedeemRewardCommandValidator()
    {
        RuleFor(x => x.RewardId)
            .NotEmpty().WithMessage("El ID de la recompensa es requerido.");
    }
}
