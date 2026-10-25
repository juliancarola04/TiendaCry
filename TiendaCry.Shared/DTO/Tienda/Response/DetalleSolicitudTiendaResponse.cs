namespace TiendaCry.Shared.DTO.Tienda.Response;

public record DetalleSolicitudTiendaResponse(int Id, string Nombre, string Calle, string NumCalle, int IdProvincia, int IdCiudad, int IdCategoria, string? MotivoRechazo);
