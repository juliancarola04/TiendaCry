using TiendaCry.Backend.Data;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Implementacion;

public class ImagenRepositoryPsqlEF : IImagenRepository
{
    private readonly DataContext _dataContext;

    public ImagenRepositoryPsqlEF(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task Guardar(Imagen imagen)
    {
        if (imagen.Id == 0)
        {
            _dataContext.Imagenes.Add(imagen);
        }

        await _dataContext.SaveChangesAsync();
    }
}
