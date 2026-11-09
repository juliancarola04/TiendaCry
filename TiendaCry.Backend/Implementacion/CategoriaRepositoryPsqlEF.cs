using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion
{
    public class CategoriaRepositoryPsqlEF : ICategoriaRepository
    {
        private readonly DataContext _dataContext;

        public CategoriaRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Categoria>> ObtenerCategorias()
        {
            return await _dataContext.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Categoria?> BuscarPorId(int idCategoria)
        {
            return await _dataContext.Categorias
                .FirstOrDefaultAsync(c => c.Id == idCategoria);
        }

        public async Task<bool> ExisteNombre(string nombre, int? idCategoriaExcluir)
        {
            string nombreComparado = nombre.Trim().ToUpper();

            return await _dataContext.Categorias
                .AnyAsync(c => c.Nombre.Trim().ToUpper() == nombreComparado
                    && (!idCategoriaExcluir.HasValue || c.Id != idCategoriaExcluir.Value));
        }

        public async Task<bool> TieneTiendas(int idCategoria)
        {
            return await _dataContext.Tiendas
                .AnyAsync(t => t.IdCategoria == idCategoria);
        }

        public async Task Crear(Categoria categoria)
        {
            _dataContext.Categorias.Add(categoria);

            await GuardarCambios();
        }

        public async Task Eliminar(Categoria categoria)
        {
            _dataContext.Categorias.Remove(categoria);

            await GuardarCambios();
        }

        public async Task GuardarCambios()
        {
            await _dataContext.SaveChangesAsync();
        }
    }
}
