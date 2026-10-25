namespace TiendaCry.Shared.DTO.Tienda.Request;

public record SolicitarAltaTiendaRequest(string Nombre, string Calle, string NumCalle, int IdCiudad, int IdCategoria);
