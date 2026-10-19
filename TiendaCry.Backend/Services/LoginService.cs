using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Backend.Services
{
    public class LoginService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly ISesionUsuarioRepository _sesionRepository;

        public LoginService(IUsuarioRepository usuarioRepository, ITokenService tokenService, ISesionUsuarioRepository sesionRepository)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _sesionRepository = sesionRepository;
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

            var token = _tokenService.CrearToken(usuario);
            var refreshToken = _tokenService.CrearRefreshToken();
            var ahora = DateTime.UtcNow;

            var sesion = new SesionUsuario
            {
                IdUsuario = usuario.Id,
                RefreshToken = _tokenService.CalcularHash(refreshToken),
                FechaCreacion = ahora,
                FechaVencimiento = ahora.AddDays(30)
            };

            await _sesionRepository.Crear(sesion);

            return new LoginResponse(token, refreshToken, sesion.FechaVencimiento);
        }
    }
}
