using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Seeder
{
    public class SeederRolesTienda
    {
        private readonly DataContext _context;

        public SeederRolesTienda(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.RolesTienda.AnyAsync())
                return;

            RolTienda[] roles =
            {
                new RolTienda { Nombre = "Administrador" },
                new RolTienda { Nombre = "Empleado" }
            };

            _context.RolesTienda.AddRange(roles);

            await _context.SaveChangesAsync();
        }
    }
}
