using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion
{
    public class ProvinciaRepositoryPsqlEF : IProvinciaRepository
    {
        private readonly DataContext _dataContext;

        public ProvinciaRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Provincia>> ObtenerProvincias()
        {
            return await _dataContext.Provincias
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<List<Ciudad>> ObtenerCiudadesPorProvincia(int idProvincia)
        {
            return await _dataContext.Ciudades
                .AsNoTracking()
                .Where(c => c.ProvinciaId == idProvincia)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }
    }
}
