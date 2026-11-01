using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Backend.Utilidades;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Backend.Services
{
    public class RegisterService
    {
        private readonly IRegisterRepository _repo;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly ISesionUsuarioRepository _sesionRepository;

        public RegisterService(
            IRegisterRepository repo,
            IUsuarioRepository usuarioRepository,
            ITokenService tokenService,
            ISesionUsuarioRepository sesionRepository)
        {
            _repo = repo;
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _sesionRepository = sesionRepository;
        }

        public async Task<AutenticacionResponse> Registrarse(
            RegisterRequest request)
        {
            if (!Validaciones.EstanDatosBien(
                    request.Username,
                    request.Password,
                    request.Email,
                    request.Nombre,
                    request.Apellido))
            {
                throw new DatosLlegaronErradosException(
                    "Todos los campos son obligatorios.");
            }

            string username = request.Username.Trim();
            string email = request.Email.Trim();
            string nombre = request.Nombre.Trim();
            string apellido = request.Apellido.Trim();

            if (!Validaciones.CumpleLongitudMaxima(username, 30)
                || !Validaciones.CumpleLongitudMaxima(email, 254)
                || !Validaciones.CumpleLongitudMaxima(nombre, 35)
                || !Validaciones.CumpleLongitudMaxima(apellido, 35))
            {
                throw new DatosLlegaronErradosException(
                    "Alguno de los campos supera la longitud permitida.");
            }

            if (!Validaciones.EsUnEmailValido(email))
            {
                throw new DatosLlegaronErradosException(
                    "El formato del email es inválido.");
            }

            if (await _usuarioRepository.ExistePorUsername(username))
            {
                throw new RecursoExistenteException(
                    "Ya existe alguien con ese usuario.");
            }

            if (await _usuarioRepository.ExistePorEmail(email))
            {
                throw new RecursoExistenteException(
                    "Ya existe alguien con ese email.");
            }

            int? idEstadoActivo = await _repo.ObtenerIdEstadoActivo();

            if (idEstadoActivo is null)
            {
                throw new InvalidOperationException(
                    "No está configurado el estado Activo de usuario.");
            }

            Usuario usuario = new Usuario
            {
                NomUsuario = username,
                Contrasena = BCrypt.Net.BCrypt.EnhancedHashPassword(
                    request.Password),
                Email = email,
                Nombre = nombre,
                Apellido = apellido,
                IdEstadoUsuario = idEstadoActivo.Value,
                EmailVerificado = false,
                EsAdministradorTiendaCry = false,
                DebeCambiarContrasena = false
            };

            await _repo.Registrarse(usuario);

            List<string> roles = await _usuarioRepository.ObtenerRoles(usuario.Id);

            string token = _tokenService.CrearToken(usuario, roles);
            string refreshToken = _tokenService.CrearRefreshToken();
            DateTime ahora = DateTime.UtcNow;

            SesionUsuario sesion = new SesionUsuario
            {
                IdUsuario = usuario.Id,
                RefreshToken = _tokenService.CalcularHash(refreshToken),
                FechaCreacion = ahora,
                FechaVencimiento = ahora.AddDays(30)
            };

            await _sesionRepository.Crear(sesion);

            return new AutenticacionResponse(token, refreshToken);
        }
    }
}
