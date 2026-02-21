using Domain.Primitives;

namespace Domain.Errors;

public static class ZoneErrors
{
    public static readonly Error NotFound =
        new("Zone.NotFound", "La zona no fue encontrada");

    public static readonly Error AlreadyExists =
        new("Zone.AlreadyExists", "Ya existe una zona con ese nombre en esa ciudad");

    public static readonly Error FruverAlreadyAssigned =
        new("Zone.FruverAlreadyAssigned", "El fruver ya está asignado a esta zona");

    public static readonly Error FruverNotAssigned =
        new("Zone.FruverNotAssigned", "El fruver no está asignado a esta zona");

    public static readonly Error FruverNotFound =
        new("Zone.FruverNotFound", "El usuario no fue encontrado o no tiene rol de FruverAliado");
}
