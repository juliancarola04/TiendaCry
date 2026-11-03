using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface ISesionUsuarioRepository
    {
        Task Crear(SesionUsuario sesion);
        Task<List<SesionUsuario>> ObtenerSesionesSinRevocar(int idUsuario);
        Task<SesionUsuario?> BuscarPorRefreshToken(string refreshToken);
        Task GuardarCambios();
    }
}
