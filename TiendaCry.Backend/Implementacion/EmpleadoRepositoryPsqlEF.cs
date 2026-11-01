using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion;

public class EmpleadoRepositoryPsqlEF : IEmpleadoRepository
{
    private readonly DataContext _dataContext;

    public EmpleadoRepositoryPsqlEF(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<bool> EsAdministradorDeTienda(int idTienda, int idUsuario)
    {
        return await _dataContext.UsuariosTienda
            .AnyAsync(ut => ut.IdTienda == idTienda
                && ut.IdUsuario == idUsuario
                && ut.RolTienda.Nombre == "Administrador"
                && ut.EstadoUsuarioTienda.Nombre == "Activo"
                && ut.FechaBaja == null
                && ut.Tienda.EstadoTienda.Nombre == "Aprobada"
                && ut.Usuario.EstadoUsuario.Nombre == "Activo"
                && ut.Usuario.EmailVerificado
                && !ut.Usuario.DebeCambiarContrasena);
    }

    public async Task<bool> TieneVinculacionActiva(int idTienda, int idUsuario)
    {
        return await _dataContext.UsuariosTienda
            .AnyAsync(ut => ut.IdTienda == idTienda && ut.IdUsuario == idUsuario
                && ut.EstadoUsuarioTienda.Nombre == "Activo" && ut.FechaBaja == null);
    }

    public async Task<List<UsuarioTienda>> ObtenerEmpleados(int idTienda)
    {
        return await _dataContext.UsuariosTienda
            .AsNoTracking()
            .Include(ut => ut.Usuario)
            .Where(ut => ut.IdTienda == idTienda
                && ut.RolTienda.Nombre == "Empleado"
                && ut.EstadoUsuarioTienda.Nombre == "Activo"
                && ut.FechaBaja == null)
            .OrderBy(ut => ut.Usuario.Nombre)
            .ThenBy(ut => ut.Usuario.Apellido)
            .ToListAsync();
    }

    public async Task<UsuarioTienda?> BuscarEmpleadoActivo(int idTienda, int idUsuarioTienda)
    {
        return await _dataContext.UsuariosTienda
            .FirstOrDefaultAsync(ut => ut.Id == idUsuarioTienda && ut.IdTienda == idTienda
                && ut.RolTienda.Nombre == "Empleado"
                && ut.EstadoUsuarioTienda.Nombre == "Activo" && ut.FechaBaja == null);
    }

    public async Task<int?> ObtenerIdRolEmpleado()
    {
        return await _dataContext.RolesTienda.Where(r => r.Nombre == "Empleado").Select(r => (int?)r.Id).FirstOrDefaultAsync();
    }

    public async Task<int?> ObtenerIdEstadoActivo()
    {
        return await _dataContext.EstadosUsuarioTienda.Where(e => e.Nombre == "Activo").Select(e => (int?)e.Id).FirstOrDefaultAsync();
    }

    public async Task<int?> ObtenerIdEstadoBaja()
    {
        return await _dataContext.EstadosUsuarioTienda.Where(e => e.Nombre == "Baja").Select(e => (int?)e.Id).FirstOrDefaultAsync();
    }

    public async Task Agregar(UsuarioTienda vinculacion)
    {
        _dataContext.UsuariosTienda.Add(vinculacion);

        await _dataContext.SaveChangesAsync();
    }

    public async Task GuardarCambios()
    {
        await _dataContext.SaveChangesAsync();
    }
}
