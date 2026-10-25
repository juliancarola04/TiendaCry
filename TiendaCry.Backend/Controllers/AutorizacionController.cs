using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AutorizacionController : ControllerBase
    {
        private readonly LoginService _loginService;
        private readonly RegisterService _registerService;
        private readonly UsuarioService _usuarioService;
        private readonly VerificacionEmailService _verificacionEmailService;
        private readonly SesionService _sesionService;
        private readonly RecuperacionContrasenaService _recuperacionContrasenaService;

        public AutorizacionController(
            LoginService loginService,
            RegisterService registerService,
            UsuarioService usuarioService,
            VerificacionEmailService verificacionEmailService,
            SesionService sesionService,
            RecuperacionContrasenaService recuperacionContrasenaService)
        {
            _loginService = loginService;
            _registerService = registerService;
            _usuarioService = usuarioService;
            _verificacionEmailService = verificacionEmailService;
            _sesionService = sesionService;
            _recuperacionContrasenaService = recuperacionContrasenaService;
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion(LogoutRequest request)
        {
            try
            {
                await _sesionService.Cerrar(request.RefreshToken);
                return NoContent();
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (SesionInvalidaException e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Renovar(RefreshRequest request)
        {
            try
            {
                var response = await _sesionService.Renovar(request.RefreshToken);
                return Ok(response);
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (SesionInvalidaException e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpPost("solicitar-recuperacion-contrasena")]
        [AllowAnonymous]
        public async Task<IActionResult> SolicitarRecuperacionContrasena(SolicitarRecuperacionContrasenaRequest request)
        {
            try
            {
                await _recuperacionContrasenaService.SolicitarRecuperacion(request);

                return Ok("Si el correo corresponde a un usuario habilitado, vas a recibir un link para restablecer tu contraseña.");
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("restablecer-contrasena")]
        [AllowAnonymous]
        public async Task<IActionResult> RestablecerContrasena(RestablecerContrasenaRequest request)
        {
            try
            {
                await _recuperacionContrasenaService.RestablecerContrasena(request);

                return Ok("La contraseña fue restablecida. Iniciá sesión nuevamente.");
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("confirmar-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarEmail(
            ConfirmarEmailRequest request)
        {
            try
            {
                await _verificacionEmailService.ConfirmarEmailAsync(request.Token);

                return Ok("El correo electrónico fue verificado.");
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("enviar-verificacion-email")]
        [Authorize]
        public async Task<IActionResult> EnviarVerificacionEmail()
        {
            try
            {
                var sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                await _verificacionEmailService.EnviarVerificacionAsync(idUsuario);

                return Ok("Se envió el correo de verificación.");
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpPut("email-sin-verificar")]
        [Authorize]
        public async Task<IActionResult> ModificarEmailSinVerificar(ModificarEmailRequest request)
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                await _usuarioService.ModificarEmailSinVerificar(idUsuario, request);

                return NoContent();
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpGet("usuario-actual")]
        [Authorize]
        public async Task<ActionResult<UsuarioActualResponse>> ObtenerUsuarioActual()
        {
            try
            {
                var sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                var response = await _usuarioService.ObtenerActual(idUsuario);

                return Ok(response);
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResponse>> Register(
            RegisterRequest registerRequest)
        {
            try
            {
                var response = await _registerService.Registrarse(registerRequest);

                return Ok(response);
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login(
            LoginRequest loginRequest)
        {
            try
            {
                var response = await _loginService.Login(loginRequest);
                return Ok(response);
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
            }
        }
    }
}
