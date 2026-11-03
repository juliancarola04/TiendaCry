using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion;

public class PromocionRepositoryPsqlEF : IPromocionRepository
{
    private readonly DataContext _dataContext;

    public PromocionRepositoryPsqlEF(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<List<Promocion>> ObtenerVigentes(int idTienda, DateTime ahora)
    {
        return await _dataContext.Promociones
            .AsNoTracking()
            .Include(p => p.Imagen)
            .Where(p => p.IdTienda == idTienda
                && p.Tienda.EstadoTienda.Nombre == "Aprobada"
                && p.EstadoPromocion.Nombre == "Aprobada"
                && p.FechaInicio <= ahora
                && p.FechaFin > ahora)
            .OrderBy(p => p.Titulo)
            .ThenBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<List<Promocion>> ObtenerAprobadas(int idTienda, DateTime ahora)
    {
        return await _dataContext.Promociones
            .AsNoTracking()
            .Include(p => p.Imagen)
            .Include(p => p.Tienda)
            .Where(p => p.IdTienda == idTienda
                && p.Tienda.EstadoTienda.Nombre == "Aprobada"
                && p.EstadoPromocion.Nombre == "Aprobada"
                && p.FechaFin > ahora)
            .OrderBy(p => p.Titulo)
            .ThenBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<List<Promocion>> ObtenerPendientes(DateTime ahora)
    {
        return await _dataContext.Promociones
            .AsNoTracking()
            .Include(p => p.Imagen)
            .Include(p => p.Tienda)
            .Where(p => p.Tienda.EstadoTienda.Nombre == "Aprobada"
                && p.EstadoPromocion.Nombre == "PendienteRevision"
                && p.FechaFin > ahora)
            .OrderBy(p => p.FechaSolicitud)
            .ThenBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<Promocion?> BuscarParaAdministracion(int idPromocion)
    {
        return await _dataContext.Promociones
            .Include(p => p.EstadoPromocion)
            .Include(p => p.Tienda)
                .ThenInclude(t => t.EstadoTienda)
            .FirstOrDefaultAsync(p => p.Id == idPromocion);
    }

    public async Task<int?> ObtenerIdEstado(string nombre)
    {
        return await _dataContext.EstadosPromocion
            .Where(e => e.Nombre == nombre)
            .Select(e => (int?)e.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<Promocion?> ObtenerDetalle(int idTienda, int idPromocion)
    {
        return await _dataContext.Promociones
            .AsNoTracking()
            .Include(p => p.Imagen)
            .FirstOrDefaultAsync(p => p.Id == idPromocion && p.IdTienda == idTienda);
    }

    public async Task<List<Promocion>> ObtenerPorTienda(int idTienda)
    {
        return await _dataContext.Promociones
            .AsNoTracking()
            .Include(p => p.Imagen)
            .Include(p => p.EstadoPromocion)
            .Where(p => p.IdTienda == idTienda)
            .OrderByDescending(p => p.FechaSolicitud)
            .ThenByDescending(p => p.Id)
            .ToListAsync();
    }

    public async Task<Dictionary<int, int>> ObtenerCantidadUsos(int idTienda)
    {
        return await _dataContext.UsosPromocion
            .Where(u => u.Promocion.IdTienda == idTienda)
            .GroupBy(u => u.IdPromocion)
            .Select(g => new
            {
                IdPromocion = g.Key,
                Cantidad = g.Count()
            })
            .ToDictionaryAsync(g => g.IdPromocion, g => g.Cantidad);
    }

    public async Task<int?> ObtenerIdEstadoPendiente()
    {
        return await _dataContext.EstadosPromocion
            .Where(e => e.Nombre == "PendienteRevision")
            .Select(e => (int?)e.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<int?> ObtenerIdEstadoCancelada()
    {
        return await _dataContext.EstadosPromocion
            .Where(e => e.Nombre == "Cancelada")
            .Select(e => (int?)e.Id)
            .FirstOrDefaultAsync();
    }

    public async Task GuardarCambios()
    {
        await _dataContext.SaveChangesAsync();
    }

    public async Task CrearSolicitud(Promocion promocion)
    {
        _dataContext.Promociones.Add(promocion);

        await _dataContext.SaveChangesAsync();
    }

    public async Task<Promocion?> BuscarPromocionAdministrable(int idPromocion, int idUsuario)
    {
        return await _dataContext.Promociones
            .Include(p => p.Imagen)
            .Include(p => p.EstadoPromocion)
            .FirstOrDefaultAsync(p => p.Id == idPromocion
                && p.Tienda.EstadoTienda.Nombre == "Aprobada"
                && p.Tienda.UsuariosTienda.Any(ut => ut.IdUsuario == idUsuario
                    && ut.RolTienda.Nombre == "Administrador"
                    && ut.EstadoUsuarioTienda.Nombre == "Activo"
                    && ut.FechaBaja == null
                    && ut.Usuario.EstadoUsuario.Nombre == "Activo"
                    && ut.Usuario.EmailVerificado
                    && !ut.Usuario.DebeCambiarContrasena));
    }
}
