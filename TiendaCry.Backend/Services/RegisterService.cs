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

        public RegisterService(
            IRegisterRepository repo,
            IUsuarioRepository usuarioRepository,
            ITokenService tokenService)
        {
            _repo = repo;
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        public async Task<RegisterResponse> Registrarse(
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

            var username = request.Username.Trim();
            var email = request.Email.Trim();
            var nombre = request.Nombre.Trim();
            var apellido = request.Apellido.Trim();

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

            var idEstadoActivo = await _repo.ObtenerIdEstadoActivo();

            if (idEstadoActivo is null)
            {
                throw new InvalidOperationException(
                    "No está configurado el estado Activo de usuario.");
            }

            var usuario = new Usuario
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

            var (token, _) = _tokenService.CrearToken(usuario);

            return new RegisterResponse(token);
        }
    }
}
