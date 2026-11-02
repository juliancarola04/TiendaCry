namespace TiendaCry.Shared.DTO.Promocion.Response;

public record PromocionListadoResponse(int Id, string Titulo, string Estado, string? UrlImagen, int CantidadUsos, bool PuedeCancelar, bool Vencida);
