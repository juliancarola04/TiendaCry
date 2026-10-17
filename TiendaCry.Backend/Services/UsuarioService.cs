using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Backend.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioActualResponse> ObtenerActual(int id)
        {
            var usuario = await _usuarioRepository.BuscarPorId(id);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            return new UsuarioActualResponse(
                usuario.Id,
                usuario.NomUsuario,
                usuario.Email,
                usuario.Nombre,
                usuario.Apellido,
                usuario.EmailVerificado,
                usuario.EsAdministradorTiendaCry,
                usuario.DebeCambiarContrasena);
        }
    }
}
