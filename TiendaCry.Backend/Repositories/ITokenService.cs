using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface ITokenService
    {
        string CrearToken(Usuario usuario, List<string> roles);
        string CrearRefreshToken();
        string CalcularHash(string token);
    }
}
