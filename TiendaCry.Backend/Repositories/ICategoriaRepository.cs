using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface ICategoriaRepository
    {
        Task<List<Categoria>> ObtenerCategorias();
        Task<Categoria?> BuscarPorId(int idCategoria);
        Task<bool> ExisteNombre(string nombre, int? idCategoriaExcluir);
        Task<bool> TieneTiendas(int idCategoria);
        Task Crear(Categoria categoria);
        Task Eliminar(Categoria categoria);
        Task GuardarCambios();
    }
}
