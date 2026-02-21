using Domain.Primitives;

namespace Domain.Errors;

public static class FruverProductErrors
{
    public static readonly Error NotFound =
        new("FruverProduct.NotFound", "El producto del fruver no fue encontrado");

    public static readonly Error AlreadyExists =
        new("FruverProduct.AlreadyExists", "Este fruver ya tiene ese producto publicado");

    public static readonly Error InsufficientStock =
        new("FruverProduct.InsufficientStock", "Stock insuficiente para completar la operación");

    public static readonly Error NotOwner =
        new("FruverProduct.NotOwner", "No tienes permiso para modificar este producto");

    public static readonly Error FruverNotActive =
        new("FruverProduct.FruverNotActive", "El fruver no está activo");
}
