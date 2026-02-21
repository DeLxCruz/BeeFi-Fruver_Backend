using Domain.Primitives;

namespace Domain.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound =
        new("Product.NotFound", "El producto no fue encontrado");

    public static readonly Error AlreadyExists =
        new("Product.AlreadyExists", "Ya existe un producto con ese nombre en esta categoría");

    public static readonly Error InactiveProduct =
        new("Product.Inactive", "El producto no está disponible");
}
