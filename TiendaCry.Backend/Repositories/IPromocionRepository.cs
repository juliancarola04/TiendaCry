using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories;

public interface IPromocionRepository
{
    Task<List<Promocion>> ObtenerPorTienda(int idTienda);
    Task<List<Promocion>> ObtenerActivasParaSocio(int? idProvincia, int? idCiudad, DateTime ahora);
    Task<Promocion?> ObtenerDetalleActivoParaSocio(int idPromocion, DateTime ahora);
    Task<List<Promocion>> ObtenerVigentes(int idTienda, DateTime ahora);
    Task<Promocion?> BuscarVigente(int idTienda, int idPromocion, DateTime ahora);
    Task<List<Promocion>> ObtenerAprobadas(int idTienda, DateTime ahora);
    Task<List<Promocion>> ObtenerPendientes(DateTime ahora);
    Task<Promocion?> BuscarParaAdministracion(int idPromocion);
    Task<int?> ObtenerIdEstado(string nombre);
    Task<Promocion?> ObtenerDetalle(int idTienda, int idPromocion);
    Task<Dictionary<int, int>> ObtenerCantidadUsos(int idTienda);
    Task<int?> ObtenerIdEstadoPendiente();
    Task CrearSolicitud(Promocion promocion);
    Task<int?> ObtenerIdEstadoCancelada();
    Task GuardarCambios();
    Task<Promocion?> BuscarPromocionAdministrable(int idPromocion, int idUsuario);
}
