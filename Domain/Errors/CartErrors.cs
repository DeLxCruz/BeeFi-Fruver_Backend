using Domain.Primitives;

namespace Domain.Errors;

public static class CartErrors
{
    public static readonly Error NotFound =
        new("Cart.NotFound", "El item del carrito no fue encontrado");

    public static readonly Error Empty =
        new("Cart.Empty", "El carrito está vacío");

    public static readonly Error FruverMismatch =
        new("Cart.FruverMismatch", "No puedes mezclar productos de diferentes fruvers. Vacía el carrito primero o usa el checkout multi-fruver");

    public static readonly Error ProductUnavailable =
        new("Cart.ProductUnavailable", "El producto no está disponible en este momento");

    public static readonly Error InsufficientStock =
        new("Cart.InsufficientStock", "No hay suficiente stock disponible");
}
