using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.Auth;
using TiendaCry.Shared.DTO.Promocion.Request;

namespace TiendaCry.Backend.Controllers;

[ApiController]
[Route("api/administracion/promociones")]
[Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
public class AdministracionPromocionesController : ControllerBase
{
    private readonly PromocionService _promocionService;

    public AdministracionPromocionesController(PromocionService promocionService)
    {
        _promocionService = promocionService;
    }

    [HttpGet("tiendas/{idTienda:int}")]
    public async Task<IActionResult> ObtenerAprobadas(int idTienda)
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            return Ok(await _promocionService.ObtenerAprobadas(idTienda, idUsuario, baseUrl));
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

    [HttpGet("pendientes")]
    public async Task<IActionResult> ObtenerPendientes()
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            return Ok(await _promocionService.ObtenerPendientes(idUsuario, baseUrl));
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

    [HttpPut("tiendas/{idTienda:int}/{idPromocion:int}/revision")]
    public async Task<IActionResult> Revisar(int idTienda, int idPromocion, RevisarPromocionRequest request)
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            bool revisada = await _promocionService.Revisar(idTienda, idPromocion, idUsuario, request);

            if (!revisada)
            {
                return NotFound("La promoción no existe.");
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

    [HttpPut("tiendas/{idTienda:int}/{idPromocion:int}/cancelar")]
    public async Task<IActionResult> Cancelar(int idTienda, int idPromocion)
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            bool cancelada = await _promocionService.CancelarComoAdministrador(idTienda, idPromocion, idUsuario);

            if (!cancelada)
            {
                return NotFound("La promoción no existe.");
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
