using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories;

public interface IEmpleadoRepository
{
    Task<bool> EsAdministradorDeTienda(int idTienda, int idUsuario);
    Task<bool> TieneVinculacionActiva(int idTienda, int idUsuario);
    Task<List<UsuarioTienda>> ObtenerEmpleados(int idTienda);
    Task<UsuarioTienda?> BuscarEmpleadoActivo(int idTienda, int idUsuarioTienda);
    Task<int?> ObtenerIdRolEmpleado();
    Task<int?> ObtenerIdEstadoActivo();
    Task<int?> ObtenerIdEstadoBaja();
    Task Agregar(UsuarioTienda vinculacion);
    Task GuardarCambios();
}
