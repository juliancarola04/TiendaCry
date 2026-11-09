using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion;

public class SocioRepositoryPsqlEF : ISocioRepository
{
    private readonly DataContext _dataContext;

    public SocioRepositoryPsqlEF(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<Socio?> BuscarPorUsuario(int idUsuario)
    {
        return await _dataContext.Socios
            .Include(s => s.Ciudad)
            .Include(s => s.Usuario)
                .ThenInclude(u => u.EstadoUsuario)
            .FirstOrDefaultAsync(s => s.IdUsuario == idUsuario);
    }

    public async Task<bool> CiudadPerteneceAProvincia(int idCiudad, int idProvincia)
    {
        return await _dataContext.Ciudades
            .AnyAsync(c => c.Id == idCiudad && c.ProvinciaId == idProvincia);
    }

    public async Task Crear(Socio socio)
    {
        _dataContext.Socios.Add(socio);

        await _dataContext.SaveChangesAsync();
    }

    public async Task GuardarCambios()
    {
        await _dataContext.SaveChangesAsync();
    }
}
