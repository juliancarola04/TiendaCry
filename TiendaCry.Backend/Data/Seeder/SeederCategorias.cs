using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Data.Seeder
{
    public class SeederCategorias
    {
        private readonly DataContext _context;

        public SeederCategorias(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Categorias.AnyAsync())
                return;

            Categoria[] categorias =
            {
                new Categoria { Nombre = "Indumentaria y calzado" },
                new Categoria { Nombre = "Gastronomía" },
                new Categoria { Nombre = "Belleza y cuidado personal" },
                new Categoria { Nombre = "Deportes y actividad física" },
                new Categoria { Nombre = "Salud y bienestar" },
                new Categoria { Nombre = "Tecnología y accesorios" },
                new Categoria { Nombre = "Hogar y decoración" },
                new Categoria { Nombre = "Entretenimiento" },
                new Categoria { Nombre = "Otros" }
            };

            _context.Categorias.AddRange(categorias);

            await _context.SaveChangesAsync();
        }
    }
}
