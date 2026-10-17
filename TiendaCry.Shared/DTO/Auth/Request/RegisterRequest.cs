namespace TiendaCry.Shared.DTO.Auth.Request;

public record RegisterRequest(
    string Username,
    string Password,
    string Email,
    string Nombre,
    string Apellido);
