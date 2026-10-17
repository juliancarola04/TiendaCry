using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Backend.Services
{
    public class LoginService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;

        public LoginService(
            IUsuarioRepository usuarioRepository,
            ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username)
                || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new DatosLlegaronErradosException(
                    "El usuario y la contraseña son obligatorios.");
            }

            var usuario = await _usuarioRepository
                .BuscarPorUsername(request.Username.Trim());

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            var contrasenaCorrecta = BCrypt.Net.BCrypt.EnhancedVerify(
                request.Password,
                usuario.Contrasena);

            if (!contrasenaCorrecta
                || usuario.EstadoUsuario.Nombre != "Activo")
            {
                throw new CredencialesInvalidasException();
            }

            var (token, _) = _tokenService.CrearToken(usuario);

            return new LoginResponse(token);
        }
    }
}
