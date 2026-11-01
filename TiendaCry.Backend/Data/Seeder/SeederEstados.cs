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
            await SeedEstadosCuponAsync();
            await SeedEstadosUsuarioTiendaAsync();
            await SeedEstadosTiendaAsync();
            await SeedEstadosPromocionAsync();
        }

        private async Task SeedEstadosUsuarioAsync()
        {
            if (await _context.EstadosUsuario.AnyAsync())
                return;

            EstadoUsuario[] estados = new EstadoUsuario[]
            {
                new EstadoUsuario { Nombre = "Activo" },
                new EstadoUsuario { Nombre = "Baja" }
            };

            _context.EstadosUsuario.AddRange(estados);

            await _context.SaveChangesAsync();
        }

        private async Task SeedEstadosCuponAsync()
        {
            if (await _context.EstadosCupon.AnyAsync())
                return;

            EstadoCupon[] estados = new EstadoCupon[]
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

            EstadoUsuarioTienda[] estados = new EstadoUsuarioTienda[]
            {
                new EstadoUsuarioTienda { Nombre = "Activo" },
                new EstadoUsuarioTienda { Nombre = "Baja" }
            };

            _context.EstadosUsuarioTienda.AddRange(estados);

            await _context.SaveChangesAsync();
        }

        private async Task SeedEstadosTiendaAsync()
        {
            if (await _context.EstadosTienda.AnyAsync())
                return;

            EstadoTienda[] estados = new EstadoTienda[]
            {
                new EstadoTienda { Nombre = "PendienteRevision" },
                new EstadoTienda { Nombre = "Aprobada" },
                new EstadoTienda { Nombre = "Rechazada" },
                new EstadoTienda { Nombre = "Baja" }
            };

            _context.EstadosTienda.AddRange(estados);

            await _context.SaveChangesAsync();
        }

        private async Task SeedEstadosPromocionAsync()
        {
            if (await _context.EstadosPromocion.AnyAsync())
                return;

            EstadoPromocion[] estados = new EstadoPromocion[]
            {
                new EstadoPromocion { Nombre = "PendienteRevision" },
                new EstadoPromocion { Nombre = "Aprobada" },
                new EstadoPromocion { Nombre = "Rechazada" },
                new EstadoPromocion { Nombre = "Cancelada" }
            };

            _context.EstadosPromocion.AddRange(estados);

            await _context.SaveChangesAsync();
        }
    }
}
