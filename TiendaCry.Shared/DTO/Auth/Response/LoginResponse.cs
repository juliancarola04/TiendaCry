namespace TiendaCry.Shared.DTO.Auth.Response;

public record LoginResponse(string Token, string RefreshToken, DateTime SesionExpira);
