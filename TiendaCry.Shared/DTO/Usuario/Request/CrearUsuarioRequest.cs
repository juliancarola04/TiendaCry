namespace TiendaCry.Shared.DTO.Usuario.Request;

public record CrearUsuarioRequest(string Usuario, string Email, string ContrasenaTemporal, string Nombre, string Apellido);
