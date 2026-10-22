using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion
{
    public class TokenEmailRepositoryPsqlEF : ITokenEmailRepository
    {
        private readonly DataContext _dataContext;

        public TokenEmailRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<int?> ObtenerIdTipoPorNombre(string nombre)
        {
            return await _dataContext.TiposTokenEmail
                .Where(t => t.Nombre == nombre)
                .Select(t => (int?)t.Id)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> ConfirmarEmailAsync(
            string codigo,
            DateTime ahora)
        {
            var token = await _dataContext.TokensEmail
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t =>
                    t.Codigo == codigo
                    && t.TipoTokenEmail.Nombre == "VerificacionEmail"
                    && !t.Usado
                    && t.FechaVencimiento > ahora
                    && !t.Usuario.EmailVerificado
                    && t.Usuario.EstadoUsuario.Nombre == "Activo");

            if (token is null)
            {
                return false;
            }

            token.Usado = true;
            token.Usuario.EmailVerificado = true;

            try
            {
                await _dataContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                _dataContext.ChangeTracker.Clear();
                return false;
            }
        }

        public async Task<bool> RestablecerContrasena(string codigo, string contrasena, DateTime ahora)
        {
            TokenEmail? token = await _dataContext.TokensEmail
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t =>
                    t.Codigo == codigo
                    && t.TipoTokenEmail.Nombre == "RecuperacionContrasena"
                    && !t.Usado
                    && t.FechaVencimiento > ahora
                    && t.Usuario.EmailVerificado
                    && t.Usuario.EstadoUsuario.Nombre == "Activo");

            if (token is null)
            {
                return false;
            }

            List<SesionUsuario> sesiones = await _dataContext.SesionesUsuario
                .Where(s => s.IdUsuario == token.IdUsuario && s.FechaRevocacion == null)
                .ToListAsync();

            token.Usuario.Contrasena = contrasena;
            token.Usuario.DebeCambiarContrasena = false;
            token.Usado = true;

            foreach (SesionUsuario sesion in sesiones)
            {
                sesion.FechaRevocacion = ahora;
            }

            try
            {
                await _dataContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                _dataContext.ChangeTracker.Clear();
                return false;
            }
        }

        public async Task GuardarAsync(TokenEmail token)
        {
            var anteriores = await _dataContext.TokensEmail
                .Where(t => t.IdUsuario == token.IdUsuario
                    && t.IdTipoTokenEmail == token.IdTipoTokenEmail
                    && !t.Usado
                    && t.FechaVencimiento > token.FechaCreacion)
                .ToListAsync();

            foreach (var anterior in anteriores)
            {
                anterior.FechaVencimiento = token.FechaCreacion;
            }

            _dataContext.TokensEmail.Add(token);

            await _dataContext.SaveChangesAsync();
        }
    }
}
