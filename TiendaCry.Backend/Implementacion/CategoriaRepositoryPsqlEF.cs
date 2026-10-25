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
    }
}
