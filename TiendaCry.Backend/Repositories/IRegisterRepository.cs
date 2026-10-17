using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface IRegisterRepository
    {
        Task Registrarse(Usuario usuario);
        Task<int?> ObtenerIdEstadoActivo();
    }
}
