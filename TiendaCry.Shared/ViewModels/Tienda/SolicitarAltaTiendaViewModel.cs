namespace TiendaCry.Shared.ViewModels.Tienda;

public class SolicitarAltaTiendaViewModel
{
    public string Nombre { get; set; } = string.Empty;
    public string Calle { get; set; } = string.Empty;
    public string NumCalle { get; set; } = string.Empty;
    public int? IdProvincia { get; set; }
    public int? IdCiudad { get; set; }
    public int? IdCategoria { get; set; }
}
