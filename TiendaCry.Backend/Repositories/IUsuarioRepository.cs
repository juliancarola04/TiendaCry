using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface IUsuarioRepository
    {
        Task<bool> ExistePorUsername(string username);
        Task<bool> ExistePorEmail(string email);
        Task<Usuario?> BuscarPorUsername(string username);
        Task<Usuario?> BuscarPorId(int id);
        Task<bool> EstaActivo(int id);
    }
}
