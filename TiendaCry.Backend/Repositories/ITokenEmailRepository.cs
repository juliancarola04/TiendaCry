using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface ITokenEmailRepository
    {
        Task<int?> ObtenerIdTipoPorNombre(string nombre);
        Task GuardarAsync(TokenEmail token);
        Task<bool> ConfirmarEmailAsync(string codigo, DateTime ahora);
        Task<bool> RestablecerContrasena(string codigo, string contrasena, DateTime ahora);
        Task<bool> ConfirmarCambioEmail(string codigo, DateTime ahora);
    }
}
