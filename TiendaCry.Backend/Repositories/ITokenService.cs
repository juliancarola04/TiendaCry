using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface ITokenService
    {
        string CrearToken(Usuario usuario);
        string CrearRefreshToken();
        string CalcularHash(string token);
    }
}
