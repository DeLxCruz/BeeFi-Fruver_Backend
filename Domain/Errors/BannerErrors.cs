using Domain.Primitives;

namespace Domain.Errors;

public static class BannerErrors
{
    public static readonly Error NotFound =
        new("Banner.NotFound", "El banner no fue encontrado");
}
