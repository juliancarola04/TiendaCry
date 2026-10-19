using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface ISesionUsuarioRepository
    {
        Task Crear(SesionUsuario sesion);
        Task<SesionUsuario?> BuscarPorRefreshToken(string refreshToken);
        Task GuardarCambios();
    }
}
