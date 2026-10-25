using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface IProvinciaRepository
    {
        Task<List<Provincia>> ObtenerProvincias();
        Task<List<Ciudad>> ObtenerCiudadesPorProvincia(int idProvincia);
    }
}
