namespace TiendaCry.Shared.DTO.Empleado.Request;

public record AgregarEmpleadoRequest(string Email, string? Usuario, string? ContrasenaTemporal, string? Nombre, string? Apellido);
