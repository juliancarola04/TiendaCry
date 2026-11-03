using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion
{
    public class SesionUsuarioRepositoryPsqlEF : ISesionUsuarioRepository
    {
        private readonly DataContext _dataContext;

        public SesionUsuarioRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<SesionUsuario>> ObtenerSesionesSinRevocar(int idUsuario)
        {
            return await _dataContext.SesionesUsuario
                .Where(s => s.IdUsuario == idUsuario && s.FechaRevocacion == null)
                .ToListAsync();
        }

        public async Task Crear(SesionUsuario sesion)
        {
            _dataContext.SesionesUsuario.Add(sesion);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<SesionUsuario?> BuscarPorRefreshToken(string refreshToken)
        {
            return await _dataContext.SesionesUsuario
                .Include(s => s.Usuario)
                    .ThenInclude(u => u.EstadoUsuario)
                .SingleOrDefaultAsync(s => s.RefreshToken == refreshToken);
        }

        public async Task GuardarCambios()
        {
            await _dataContext.SaveChangesAsync();
        }
    }
}
