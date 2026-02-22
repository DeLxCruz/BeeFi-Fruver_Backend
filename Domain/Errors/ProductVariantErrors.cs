using Domain.Primitives;

namespace Domain.Errors;

public static class ProductVariantErrors
{
    public static readonly Error NotFound =
        new("ProductVariant.NotFound", "La variante del producto no fue encontrada");

    public static readonly Error NotOwner =
        new("ProductVariant.NotOwner", "No tienes permiso para modificar esta variante");
}
