using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.Auth;
using TiendaCry.Shared.DTO.Administrador.Request;
using TiendaCry.Shared.DTO.Usuario.Response;

namespace TiendaCry.Backend.Controllers;

[ApiController]
[Route("api/administracion/administradores")]
[Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
public class AdministradoresController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public AdministradoresController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UsuarioListadoResponse>>> ObtenerAdministradores()
    {
        try
        {
            int idAdministrador = Convert.ToInt32(User.FindFirst("sub")?.Value);

            List<UsuarioListadoResponse> response = await _usuarioService.ObtenerAdministradoresActivos(idAdministrador);

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

    [HttpPost]
    public async Task<IActionResult> CrearAdministrador(CrearAdministradorRequest request)
    {
        try
        {
            int idAdministrador = Convert.ToInt32(User.FindFirst("sub")?.Value);

            await _usuarioService.CrearAdministrador(idAdministrador, request);

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

    [HttpPut("{idAdministrador:int}/baja")]
    public async Task<IActionResult> DarDeBaja(int idAdministrador)
    {
        try
        {
            int idAdministradorActual = Convert.ToInt32(User.FindFirst("sub")?.Value);

            bool dadoDeBaja = await _usuarioService.DarDeBajaAdministrador(idAdministrador, idAdministradorActual);

            if (!dadoDeBaja)
            {
                return NotFound("El administrador no existe o ya está dado de baja.");
            }

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
}
