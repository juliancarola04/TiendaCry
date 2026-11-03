using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.Auth;
using TiendaCry.Shared.DTO.Usuario.Request;
using TiendaCry.Shared.DTO.Usuario.Response;

namespace TiendaCry.Backend.Controllers;

[ApiController]
[Route("api/administracion/usuarios")]
[Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UsuarioListadoResponse>>> ObtenerUsuarios()
    {
        try
        {
            int idAdministrador = Convert.ToInt32(User.FindFirst("sub")?.Value);

            List<UsuarioListadoResponse> response = await _usuarioService.ObtenerUsuariosActivos(idAdministrador);

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

    [HttpPut("{idUsuario:int}/baja")]
    public async Task<IActionResult> DarDeBaja(int idUsuario)
    {
        try
        {
            int idAdministrador = Convert.ToInt32(User.FindFirst("sub")?.Value);

            bool dadoDeBaja = await _usuarioService.DarDeBaja(idUsuario, idAdministrador);

            if (!dadoDeBaja)
            {
                return NotFound("El usuario no existe, ya está dado de baja o es administrador de TiendaCry.");
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
    }

    [HttpPost]
    public async Task<IActionResult> CrearUsuario(CrearUsuarioRequest request)
    {
        try
        {
            int idAdministrador = Convert.ToInt32(User.FindFirst("sub")?.Value);

            await _usuarioService.CrearUsuario(idAdministrador, request);

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
}
