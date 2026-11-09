using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Perfil.Request;
using TiendaCry.Shared.DTO.Perfil.Response;

namespace TiendaCry.Backend.Controllers;

[ApiController]
[Route("api/perfil")]
[Authorize]
public class PerfilController : ControllerBase
{
    private readonly PerfilService _perfilService;

    public PerfilController(PerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    [HttpGet]
    public async Task<ActionResult<PerfilResponse>> ObtenerPerfil()
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            PerfilResponse response = await _perfilService.ObtenerPerfil(idUsuario);

            return Ok(response);
        }
        catch (CredencialesInvalidasException)
        {
            return Unauthorized();
        }
        catch (SinPermisoException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, e.Message);
        }
    }

    [HttpPut("contrasena")]
    public async Task<IActionResult> CambiarContrasena(CambiarContrasenaRequest request)
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            await _perfilService.CambiarContrasena(idUsuario, request);

            return NoContent();
        }
        catch (CredencialesInvalidasException)
        {
            return Unauthorized();
        }
        catch (SinPermisoException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, e.Message);
        }
        catch (DatosLlegaronErradosException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("solicitar-cambio-email")]
    public async Task<IActionResult> SolicitarCambioEmail(ModificarEmailRequest request)
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            await _perfilService.SolicitarCambioEmail(idUsuario, request);

            return NoContent();
        }
        catch (CredencialesInvalidasException)
        {
            return Unauthorized();
        }
        catch (SinPermisoException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, e.Message);
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

    [HttpPost("confirmar-cambio-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmarCambioEmail(ConfirmarEmailRequest request)
    {
        try
        {
            await _perfilService.ConfirmarCambioEmail(request);

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
}
