using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion
{
    public class RegisterRepositoryPsqlEF : IRegisterRepository
    {
        private readonly DataContext _dataContext;

        public RegisterRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<int?> ObtenerIdEstadoActivo()
        {
            return await _dataContext.EstadosUsuario
                .Where(e => e.Nombre == "Activo")
                .Select(e => (int?)e.Id)
                .SingleOrDefaultAsync();
        }

        public async Task Registrarse(Usuario usuario)
        {
            _dataContext.Usuarios.Add(usuario);

            await _dataContext.SaveChangesAsync();
        }
    }
}
