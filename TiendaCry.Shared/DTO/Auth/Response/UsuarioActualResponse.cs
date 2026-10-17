namespace TiendaCry.Shared.DTO.Auth.Response;

public record UsuarioActualResponse(
    int Id,
    string NomUsuario,
    string Email,
    string Nombre,
    string Apellido,
    bool EmailVerificado,
    bool EsAdministradorTiendaCry,
    bool DebeCambiarContrasena);
