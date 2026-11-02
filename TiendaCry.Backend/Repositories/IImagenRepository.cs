using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories;

public interface IImagenRepository
{
    Task Guardar(Imagen imagen);
}
