using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface ICategoriaRepository
    {
        Task<List<Categoria>> ObtenerCategorias();
    }
}
