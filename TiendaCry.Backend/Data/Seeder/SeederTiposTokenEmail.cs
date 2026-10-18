using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Seeder
{
    public class SeederTiposTokenEmail
    {
        private readonly DataContext _context;

        public SeederTiposTokenEmail(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedTiposTokenEmailAsync();
        }

        private async Task SeedTiposTokenEmailAsync()
        {
            if (await _context.TiposTokenEmail.AnyAsync())
                return;

            // Nota para Julián del futuro de cada cosa:

            /* VerificacionEmail: Cuando un nuevo usuario se crea un usuario a la hora de registrarse o cuando va hacia atrás a la hora de registrarse
                                  y luego intenta entrar con esas credenciales a logearse, o reenviar otro código o modificar el correo. => EmailVerificado = false; EmailNuevo = null;*/
            // RecuperacionContrasena: Cuando intenta recuperar contraseña. => EmailVerificado = true; EmailNuevo = null;
            // CambioEmail: Cuando un usuario ya registrado intenta cambiar su correo. => EmailVerificado = true; EmailNuevo = not null;
            var tipos = new[]
            {
                new TipoTokenEmail { Nombre = "VerificacionEmail" },
                new TipoTokenEmail { Nombre = "RecuperacionContrasena" },
                new TipoTokenEmail { Nombre = "CambioEmail" }
            };

            _context.TiposTokenEmail.AddRange(tipos);

            await _context.SaveChangesAsync();
        }
    }
}
