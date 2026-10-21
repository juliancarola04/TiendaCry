using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
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

        [HttpGet("web/csrf")]
        [AllowAnonymous]
        public IActionResult ObtenerTokenCsrf([FromServices] IAntiforgery antiforgery)
        {
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);

            return Ok(new { token = tokens.RequestToken });
        }

        [HttpPost("web/login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWeb(LoginRequest request)
        {
            try
            {
                var response = await _loginService.Login(request);

                GuardarCookies(response.Token, response.RefreshToken, response.SesionExpira);

                return NoContent();
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

        [HttpPost("web/register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterWeb(RegisterRequest request)
        {
            try
            {
                RegisterResponse response = await _registerService.Registrarse(request);

                GuardarCookies(response.Token, response.RefreshToken, response.SesionExpira);

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
        }

        [HttpPost("web/refresh")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenovarWeb()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                EliminarCookies();
                return Unauthorized("No hay una sesión disponible para renovar.");
            }

            try
            {
                var response = await _sesionService.Renovar(refreshToken);

                GuardarCookies(response.Token, response.RefreshToken, response.SesionExpira);

                return NoContent();
            }
            catch (SesionInvalidaException e)
            {
                EliminarCookies();
                return Unauthorized(e.Message);
            }
        }

        [HttpPost("web/logout")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesionWeb()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            try
            {
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    await _sesionService.Cerrar(refreshToken);
                }

                EliminarCookies();

                return NoContent();
            }
            catch (SesionInvalidaException e)
            {
                return Unauthorized(e.Message);
            }
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
        private void GuardarCookies(string token, string refreshToken, DateTime sesionExpira)
        {
            JsonWebToken jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = new DateTimeOffset(jwt.ValidTo)
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth",
                Expires = new DateTimeOffset(sesionExpira)
            });
        }

        private void EliminarCookies()
        {
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth"
            });
        }
    }
}
