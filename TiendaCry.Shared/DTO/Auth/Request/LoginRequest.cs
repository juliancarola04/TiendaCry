namespace TiendaCry.Shared.DTO.Auth.Request;

public record LoginRequest(
    string Username,
    string Password);
