namespace TiendaCry.Shared.DTO.Socio.Response;

public record DetallePromocionSocioResponse(string Titulo, string Descripcion, string? UrlImagen, string Provincia, string Ciudad, string Calle, string NumCalle, DateTime FechaInicio, DateTime FechaFin);
