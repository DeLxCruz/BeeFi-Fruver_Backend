using Domain.Primitives;

namespace Domain.Errors;

public static class RewardErrors
{
    public static readonly Error NotFound =
        new("Reward.NotFound", "La recompensa no fue encontrada");

    public static readonly Error NotAvailable =
        new("Reward.NotAvailable", "La recompensa no está disponible");

    public static readonly Error AlreadyRedeemed =
        new("Reward.AlreadyRedeemed", "Esta recompensa ya fue canjeada");

    public static readonly Error Expired =
        new("Reward.Expired", "La recompensa ha expirado");

    public static readonly Error MaxRedemptionsReached =
        new("Reward.MaxRedemptionsReached", "Esta recompensa alcanzó el límite de canjes");
}
