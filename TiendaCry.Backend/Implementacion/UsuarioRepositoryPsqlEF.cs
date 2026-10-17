using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion
{
    public class UsuarioRepositoryPsqlEF : IUsuarioRepository
    {
        private readonly DataContext _dataContext;

        public UsuarioRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> ExistePorUsername(string username)
        {
            return await _dataContext.Usuarios
                .AnyAsync(u => u.NomUsuario == username);
        }

        public async Task<bool> ExistePorEmail(string email)
        {
            return await _dataContext.Usuarios
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> EstaActivo(int id)
        {
            return await _dataContext.Usuarios
                .AnyAsync(u => u.Id == id
                    && u.EstadoUsuario.Nombre == "Activo");
        }

        public async Task<Usuario?> BuscarPorId(int id)
        {
            return await _dataContext.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> BuscarPorUsername(string username)
        {
            return await _dataContext.Usuarios
                .AsNoTracking()
                .Include(u => u.EstadoUsuario)
                .FirstOrDefaultAsync(u => u.NomUsuario == username);
        }
    }
}
