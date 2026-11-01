using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface ITiendaRepository
    {
        Task<List<Tienda>> ObtenerSolicitudesPorUsuario(int idUsuario);
        Task<List<Tienda>> ObtenerSolicitudesPendientes();
        Task<List<Tienda>> ObtenerTiendasAprobadas();
        Task<Tienda?> BuscarTiendaAprobada(int idTienda);
        Task<bool> TieneTiendaComoAdministrador(int idUsuario);
        Task<Tienda?> BuscarTiendaActivaPorAdministrador(int idUsuario);
        Task<bool> ExisteCiudad(int idCiudad);
        Task<bool> ExisteCategoria(int idCategoria);
        Task<int?> ObtenerIdEstadoPendiente();
        Task<int?> ObtenerIdRolAdministrador();
        Task<int?> ObtenerIdEstadoVinculacionActivo();
        Task CrearSolicitud(Tienda tienda);

        Task<Tienda?> BuscarSolicitudRechazada(int idTienda, int idUsuario);

        Task GuardarCambios();
    }
}
