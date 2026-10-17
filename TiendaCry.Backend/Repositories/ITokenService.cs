using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface ITokenService
    {
        (string token, DateTime expiracion) CrearToken(Usuario usuario);
    }
}
