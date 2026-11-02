namespace TiendaCry.Shared.DTO.Promocion.Request;

public record SolicitarAltaPromocionRequest(int IdTienda, string Titulo, string Descripcion, DateTimeOffset FechaInicio, DateTimeOffset FechaFin);
