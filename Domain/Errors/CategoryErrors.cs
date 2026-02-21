using Domain.Primitives;

namespace Domain.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound =
        new("Category.NotFound", "La categoría no fue encontrada");

    public static readonly Error AlreadyExists =
        new("Category.AlreadyExists", "Ya existe una categoría con ese nombre en este nivel");

    public static readonly Error HasActiveProducts =
        new("Category.HasActiveProducts", "No se puede desactivar una categoría con productos activos");

    public static readonly Error ParentNotFound =
        new("Category.ParentNotFound", "La categoría padre no fue encontrada");
}
