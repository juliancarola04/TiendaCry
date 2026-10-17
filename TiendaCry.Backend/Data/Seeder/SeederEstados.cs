using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Seeder
{
    public class SeederEstados
    {
        private readonly DataContext _context;

        public SeederEstados(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedEstadosUsuarioAsync();
            await SeedEstadosSocioAsync();
            await SeedEstadosCuponAsync();
            await SeedEstadosUsuarioTiendaAsync();
            await SeedEstadosTiendaAsync();
            await SeedEstadosPromocionAsync();
        }

        private async Task SeedEstadosUsuarioAsync()
        {
            if (await _context.EstadosUsuario.AnyAsync())
                return;

            var estados = new[]
            {
                new EstadoUsuario { Nombre = "Activo" },
                new EstadoUsuario { Nombre = "Bloqueado" },
                new EstadoUsuario { Nombre = "Baja" }
            };

            _context.EstadosUsuario.AddRange(estados);

            await _context.SaveChangesAsync();
        }

        private async Task SeedEstadosSocioAsync()
        {
            if (await _context.EstadosSocio.AnyAsync())
                return;

            var estados = new[]
            {
                new EstadoSocio { Nombre = "Habilitado" },
                new EstadoSocio { Nombre = "Suspendido" },
                new EstadoSocio { Nombre = "Baja" }
            };

            _context.EstadosSocio.AddRange(estados);

            await _context.SaveChangesAsync();
        }

        private async Task SeedEstadosCuponAsync()
        {
            if (await _context.EstadosCupon.AnyAsync())
                return;

            var estados = new[]
            {
                new EstadoCupon { Nombre = "Activo" },
                new EstadoCupon { Nombre = "Utilizado" },
                new EstadoCupon { Nombre = "Cancelado" },
                new EstadoCupon { Nombre = "Vencido" }
            };

            _context.EstadosCupon.AddRange(estados);

            await _context.SaveChangesAsync();
        }

        private async Task SeedEstadosUsuarioTiendaAsync()
        {
            if (await _context.EstadosUsuarioTienda.AnyAsync())
                return;

            var estados = new[]
            {
                new EstadoUsuarioTienda { Nombre = "Activo" },
                new EstadoUsuarioTienda { Nombre = "Suspendido" },
                new EstadoUsuarioTienda { Nombre = "Baja" }
            };

            _context.EstadosUsuarioTienda.AddRange(estados);

            await _context.SaveChangesAsync();
        }

        private async Task SeedEstadosTiendaAsync()
        {
            if (await _context.EstadosTienda.AnyAsync())
                return;

            var estados = new[]
            {
                new EstadoTienda { Nombre = "PendienteRevision" },
                new EstadoTienda { Nombre = "Aprobada" },
                new EstadoTienda { Nombre = "Rechazada" },
                new EstadoTienda { Nombre = "Suspendida" },
                new EstadoTienda { Nombre = "Baja" }
            };

            _context.EstadosTienda.AddRange(estados);

            await _context.SaveChangesAsync();
        }

        private async Task SeedEstadosPromocionAsync()
        {
            if (await _context.EstadosPromocion.AnyAsync())
                return;

            var estados = new[]
            {
                new EstadoPromocion { Nombre = "PendienteRevision" },
                new EstadoPromocion { Nombre = "Aprobada" },
                new EstadoPromocion { Nombre = "Rechazada" },
                new EstadoPromocion { Nombre = "Suspendida" },
                new EstadoPromocion { Nombre = "Cancelada" }
            };

            _context.EstadosPromocion.AddRange(estados);

            await _context.SaveChangesAsync();
        }
    }
}

