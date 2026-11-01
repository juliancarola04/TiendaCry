using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.Auth;

namespace TiendaCry.Backend.Implementacion
{
    public class UsuarioRepositoryPsqlEF : IUsuarioRepository
    {
        private readonly DataContext _dataContext;

        public UsuarioRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<string>> ObtenerRoles(int idUsuario)
        {
            List<string> roles = new List<string>();

            bool esAdministradorTiendaCry = await _dataContext.Usuarios
                .AnyAsync(u => u.Id == idUsuario
                    && u.EstadoUsuario.Nombre == "Activo"
                    && u.EsAdministradorTiendaCry);

            if (esAdministradorTiendaCry)
            {
                roles.Add(RolesUsuario.AdministradorTiendaCry);
            }

            bool esSocio = await _dataContext.Socios
                .AnyAsync(s => s.IdUsuario == idUsuario
                    && s.Usuario.EstadoUsuario.Nombre == "Activo");

            if (esSocio)
            {
                roles.Add(RolesUsuario.Socio);
            }

            List<string> rolesTienda = await _dataContext.UsuariosTienda
                .Where(ut => ut.IdUsuario == idUsuario
                    && ut.Usuario.EstadoUsuario.Nombre == "Activo"
                    && ut.FechaBaja == null
                    && ut.EstadoUsuarioTienda.Nombre == "Activo"
                    && ut.Tienda.EstadoTienda.Nombre == "Aprobada"
                    && (ut.RolTienda.Nombre == "Administrador"
                        || ut.RolTienda.Nombre == "Empleado"))
                .Select(ut => ut.RolTienda.Nombre)
                .Distinct()
                .ToListAsync();

            if (rolesTienda.Contains("Administrador"))
            {
                roles.Add(RolesUsuario.AdministradorTienda);
                roles.Add(RolesUsuario.EmpleadoTienda);
            }
            else if (rolesTienda.Contains("Empleado"))
            {
                roles.Add(RolesUsuario.EmpleadoTienda);
            }

            return roles;
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

        public async Task<bool> ModificarEmailSinVerificar(int idUsuario, string email, DateTime ahora)
        {
            Usuario? usuario = await _dataContext.Usuarios
                .Include(u => u.TokensEmail.Where(t =>
                    t.TipoTokenEmail.Nombre == "VerificacionEmail"
                    && !t.Usado
                    && t.FechaVencimiento > ahora))
                .FirstOrDefaultAsync(u => u.Id == idUsuario && !u.EmailVerificado);

            if (usuario is null)
            {
                return false;
            }

            usuario.Email = email;

            foreach (TokenEmail token in usuario.TokensEmail)
            {
                token.FechaVencimiento = ahora;
            }

            await _dataContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CambiarContrasenaTemporal(int idUsuario, string contrasena)
        {
            Usuario? usuario = await _dataContext.Usuarios.FirstOrDefaultAsync(u => u.Id == idUsuario
                && u.EstadoUsuario.Nombre == "Activo" && u.EmailVerificado && u.DebeCambiarContrasena);

            if (usuario is null)
            {
                return false;
            }

            usuario.Contrasena = contrasena;
            usuario.DebeCambiarContrasena = false;

            await _dataContext.SaveChangesAsync();

            return true;
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

        public async Task<Usuario?> BuscarPorEmail(string email)
        {
            return await _dataContext.Usuarios
                .AsNoTracking()
                .Include(u => u.EstadoUsuario)
                .FirstOrDefaultAsync(u => u.Email == email);
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
