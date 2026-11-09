using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.Auth;
using TiendaCry.Shared.DTO.Socio.Request;
using TiendaCry.Shared.DTO.Socio.Response;

namespace TiendaCry.Backend.Controllers;

[ApiController]
[Route("api/socio")]
[Authorize]
public class SocioController : ControllerBase
{
    private readonly SocioService _socioService;

    public SocioController(SocioService socioService)
    {
        _socioService = socioService;
    }

    [HttpPost("alta")]
    public async Task<IActionResult> CrearSocio()
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            await _socioService.CrearSocio(idUsuario);

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

    [HttpGet("ubicacion")]
    [Authorize(Roles = RolesUsuario.Socio)]
    public async Task<ActionResult<UbicacionSocioResponse>> ObtenerUbicacion()
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            UbicacionSocioResponse response = await _socioService.ObtenerUbicacion(idUsuario);

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

    [HttpPut("ubicacion")]
    [Authorize(Roles = RolesUsuario.Socio)]
    public async Task<IActionResult> GuardarUbicacion(GuardarUbicacionRequest request)
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            await _socioService.GuardarUbicacion(idUsuario, request);

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

    [HttpGet("promociones")]
    [Authorize(Roles = RolesUsuario.Socio)]
    public async Task<ActionResult<List<PromocionSocioResponse>>> ObtenerPromociones([FromQuery] bool cerca = false)
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            List<PromocionSocioResponse> response = await _socioService.ObtenerPromociones(idUsuario, cerca, baseUrl);

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
        catch (DatosLlegaronErradosException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("promociones/{idPromocion:int}")]
    [Authorize(Roles = RolesUsuario.Socio)]
    public async Task<ActionResult<DetallePromocionSocioResponse>> ObtenerDetalle(int idPromocion)
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            DetallePromocionSocioResponse? response = await _socioService.ObtenerDetalle(idUsuario, idPromocion, baseUrl);

            if (response is null)
            {
                return NotFound("La promoción no existe o ya no está vigente.");
            }

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
}
