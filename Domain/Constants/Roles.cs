namespace Domain.Constants;

public static class Roles
{
    public const string Cliente = "Cliente";
    public const string Empleado = "Empleado";
    public const string Administrador = "Administrador";
    public const string FruverAliado = "FruverAliado";
    public const string AdminOrEmpleado = Administrador + "," + Empleado;
}
