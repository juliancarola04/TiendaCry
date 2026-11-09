using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> ObtenerUsuariosActivos();
        Task<List<Usuario>> ObtenerAdministradoresActivos();
        Task<Usuario?> BuscarAdministradorActivoParaBaja(int idAdministrador);
        Task<int> ContarAdministradoresActivos();
        Task<Usuario?> BuscarUsuarioActivoParaBaja(int idUsuario);
        Task<int?> ObtenerIdEstadoBaja();
        Task GuardarCambios();
        Task<bool> ExistePorUsername(string username);
        Task<bool> ExistePorEmail(string email);
        Task<Usuario?> BuscarPorUsername(string username);
        Task<Usuario?> BuscarPorEmail(string email);
        Task<Usuario?> BuscarPorId(int id);
        Task<bool> CambiarContrasenaTemporal(int idUsuario, string contrasena);
        Task<bool> CambiarContrasena(int idUsuario, string contrasena, DateTime ahora);
        Task<bool> EstaActivo(int id);
        Task<List<string>> ObtenerRoles(int idUsuario);
        Task<bool> ModificarEmailSinVerificar(int idUsuario, string email, DateTime ahora);
    }
}
