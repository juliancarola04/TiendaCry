using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;

namespace TiendaCry.Backend.Controllers
{
    [Route("api/email")]
    [ApiController]
    [Authorize]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly UsuarioService _usuarioService;

        public EmailController(
            EmailService emailService,
            UsuarioService usuarioService)
        {
            _emailService = emailService;
            _usuarioService = usuarioService;
        }

        [HttpPost("prueba")]
        public async Task<IActionResult> EnviarPrueba()
        {
            try
            {
                var sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                var usuario = await _usuarioService.ObtenerActual(idUsuario);

                await _emailService.EnviarAsync(
                    usuario.Email,
                    "Correo de prueba de TiendaCry",
                    "<h1>TiendaCry</h1><p>El envío de correos funciona.</p>");

                return Ok("El servidor SMTP aceptó el correo para su envío.");
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
            }
        }
    }
}
