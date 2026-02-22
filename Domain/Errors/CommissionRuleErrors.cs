using Domain.Primitives;

namespace Domain.Errors;

public static class CommissionRuleErrors
{
    public static readonly Error NotFound =
        new("CommissionRule.NotFound", "La regla de comisión no fue encontrada");
}
