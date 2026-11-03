namespace TiendaCry.Shared.DTO.Promocion.Response;

public record PromocionesTiendaEmpleadoResponse(int IdTienda, string NombreTienda, List<PromocionEmpleadoResponse> Promociones);
