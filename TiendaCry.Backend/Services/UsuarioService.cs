using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Repositories;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Utilidades;
using TiendaCry.Shared.DTO.Auth.Request;

namespace TiendaCry.Backend.Services
{
    public class UsuarioService
    {
        private readonly ILogger<UsuarioService> _logger;
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
        }

        public async Task CambiarContrasenaTemporal(int idUsuario, CambiarContrasenaTemporalRequest request)
        {
            if (!Validaciones.EstanDatosBien(request.Contrasena))
            {
                throw new DatosLlegaronErradosException("Ingresá tu contraseña nueva.");
            }

            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null || !await _usuarioRepository.EstaActivo(idUsuario))
            {
                throw new CredencialesInvalidasException();
            }

            if (!usuario.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo antes de cambiar la contraseña temporal.");
            }

            if (!usuario.DebeCambiarContrasena)
            {
                throw new DatosLlegaronErradosException("La cuenta no requiere un cambio de contraseña temporal.");
            }

            string contrasena = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Contrasena);

            if (!await _usuarioRepository.CambiarContrasenaTemporal(idUsuario, contrasena))
            {
                throw new SinPermisoException("No se pudo cambiar la contraseña temporal de esta cuenta.");
            }

            _logger.LogInformation("El usuario {IdUsuario} cambió su contraseña temporal.", idUsuario);
        }

        public async Task ModificarEmailSinVerificar(int idUsuario, ModificarEmailRequest request)
        {
            if (!Validaciones.EstanDatosBien(request.Email))
            {
                throw new DatosLlegaronErradosException("Ingresá el nuevo correo.");
            }

            string email = request.Email.Trim();

            if (!Validaciones.CumpleLongitudMaxima(email, 254) || !Validaciones.EsUnEmailValido(email))
            {
                throw new DatosLlegaronErradosException("El correo ingresado no es válido.");
            }

            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (usuario.EmailVerificado)
            {
                throw new DatosLlegaronErradosException("Este cambio solo está disponible para correos sin verificar.");
            }

            if (usuario.Email == email)
            {
                throw new DatosLlegaronErradosException("Ingresá un correo diferente al actual.");
            }

            if (await _usuarioRepository.ExistePorEmail(email))
            {
                throw new RecursoExistenteException("Ya existe una cuenta con ese correo.");
            }

            bool modificado = await _usuarioRepository.ModificarEmailSinVerificar(idUsuario, email, DateTime.UtcNow);

            if (!modificado)
            {
                throw new DatosLlegaronErradosException("No se pudo modificar el correo de esta cuenta.");
            }

            _logger.LogInformation("El usuario {IdUsuario} modificó su correo sin verificar.", idUsuario);
        }

    }
}
