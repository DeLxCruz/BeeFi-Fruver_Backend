using Domain.Primitives;

namespace Domain.Errors;

public static class LoyaltyErrors
{
    public static readonly Error NotFound =
        new("Loyalty.NotFound", "Puntos de lealtad no encontrados");

    public static readonly Error InsufficientPoints =
        new("Loyalty.InsufficientPoints", "Puntos insuficientes para canjear esta recompensa");
}
