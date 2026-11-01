using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion
{
    public class TiendaRepositoryPsqlEF : ITiendaRepository
    {
        private readonly DataContext _dataContext;

        public TiendaRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Tienda>> ObtenerSolicitudesPorUsuario(int idUsuario)
        {
            return await _dataContext.Tiendas
                .AsNoTracking()
                .Include(t => t.EstadoTienda)
                .Where(t => t.EstadoTienda.Nombre == "PendienteRevision"
                    || t.EstadoTienda.Nombre == "Rechazada")
                .Where(t => t.UsuariosTienda.Any(ut => ut.IdUsuario == idUsuario
                    && ut.FechaBaja == null
                    && ut.EstadoUsuarioTienda.Nombre == "Activo"
                    && ut.RolTienda.Nombre == "Administrador"))
                .OrderByDescending(t => t.FechaSolicitud)
                .ToListAsync();
        }
        public async Task<List<Tienda>> ObtenerSolicitudesPendientes()
        {
            return await _dataContext.Tiendas
                .AsNoTracking()
                .Where(t => t.EstadoTienda.Nombre == "PendienteRevision")
                .OrderBy(t => t.FechaSolicitud)
                .ThenBy(t => t.Id)
                .ToListAsync();
        }

        public async Task<List<Tienda>> ObtenerTiendasAprobadas()
        {
            return await _dataContext.Tiendas
                .AsNoTracking()
                .Where(t => t.EstadoTienda.Nombre == "Aprobada")
                .OrderBy(t => t.Nombre)
                .ThenBy(t => t.Id)
                .ToListAsync();
        }

        public async Task<Tienda?> BuscarTiendaAprobada(int idTienda)
        {
            return await _dataContext.Tiendas
                .AsNoTracking()
                .Include(t => t.Ciudad)
                    .ThenInclude(c => c.Provincia)
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == idTienda && t.EstadoTienda.Nombre == "Aprobada");
        }

        public async Task<bool> TieneTiendaComoAdministrador(int idUsuario)
        {
            return await _dataContext.UsuariosTienda
                .AnyAsync(ut => ut.IdUsuario == idUsuario
                    && ut.RolTienda.Nombre == "Administrador"
                    && ut.FechaBaja == null);
        }

        public async Task<Tienda?> BuscarTiendaActivaPorAdministrador(int idUsuario)
        {
            return await _dataContext.Tiendas
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.EstadoTienda.Nombre == "Aprobada"
                    && t.UsuariosTienda.Any(ut => ut.IdUsuario == idUsuario
                        && ut.RolTienda.Nombre == "Administrador"
                        && ut.EstadoUsuarioTienda.Nombre == "Activo"
                        && ut.FechaBaja == null));
        }

        public async Task<bool> ExisteCiudad(int idCiudad)
        {
            return await _dataContext.Ciudades.AnyAsync(c => c.Id == idCiudad);
        }

        public async Task<bool> ExisteCategoria(int idCategoria)
        {
            return await _dataContext.Categorias.AnyAsync(c => c.Id == idCategoria);
        }

        public async Task<int?> ObtenerIdEstadoPendiente()
        {
            return await _dataContext.EstadosTienda
                .Where(e => e.Nombre == "PendienteRevision")
                .Select(e => (int?)e.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> ObtenerIdRolAdministrador()
        {
            return await _dataContext.RolesTienda
                .Where(r => r.Nombre == "Administrador")
                .Select(r => (int?)r.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> ObtenerIdEstadoVinculacionActivo()
        {
            return await _dataContext.EstadosUsuarioTienda
                .Where(e => e.Nombre == "Activo")
                .Select(e => (int?)e.Id)
                .FirstOrDefaultAsync();
        }

        public async Task CrearSolicitud(Tienda tienda)
        {
            _dataContext.Tiendas.Add(tienda);

            await _dataContext.SaveChangesAsync();
        }

        public async Task<Tienda?> BuscarSolicitudRechazada(int idTienda, int idUsuario)
        {
            return await _dataContext.Tiendas
                .Include(t => t.Ciudad)
                .FirstOrDefaultAsync(t => t.Id == idTienda
                    && t.EstadoTienda.Nombre == "Rechazada"
                    && t.UsuariosTienda.Any(ut => ut.IdUsuario == idUsuario
                        && ut.RolTienda.Nombre == "Administrador"
                        && ut.EstadoUsuarioTienda.Nombre == "Activo"
                        && ut.FechaBaja == null));
        }

        public async Task GuardarCambios()
        {
            await _dataContext.SaveChangesAsync();
        }
    }
}
