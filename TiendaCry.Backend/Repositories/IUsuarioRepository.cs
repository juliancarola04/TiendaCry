using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface IUsuarioRepository
    {
        Task<bool> ExistePorUsername(string username);
        Task<bool> ExistePorEmail(string email);
        Task<Usuario?> BuscarPorUsername(string username);
        Task<Usuario?> BuscarPorEmail(string email);
        Task<Usuario?> BuscarPorId(int id);
        Task<bool> CambiarContrasenaTemporal(int idUsuario, string contrasena);
        Task<bool> EstaActivo(int id);
        Task<List<string>> ObtenerRoles(int idUsuario);
        Task<bool> ModificarEmailSinVerificar(int idUsuario, string email, DateTime ahora);
    }
}
