namespace TiendaCry.Shared.DTO.Promocion.Response;

public record DetallePromocionResponse(string Titulo, string Descripcion, string? UrlImagen, DateTime FechaInicio, DateTime FechaFin);
