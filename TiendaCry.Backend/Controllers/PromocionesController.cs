using TiendaCry.Shared.DTO.Promocion.Request;
using TiendaCry.Shared.DTO.Promocion.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.Auth;
using TiendaCry.Shared.DTO.Imagen.Response;

namespace TiendaCry.Backend.Controllers;

[ApiController]
[Route("api/promociones")]
[Authorize]
public class PromocionesController : ControllerBase
{
    private readonly ImagenService _imagenService;
    private readonly PromocionService _promocionService;

    public PromocionesController(ImagenService imagenService, PromocionService promocionService)
    {
        _imagenService = imagenService;
        _promocionService = promocionService;
    }

    [HttpGet("tienda/{idTienda:int}/{idPromocion:int}/detalles")]
    [Authorize(Roles = RolesUsuario.AdministradorTienda + "," + RolesUsuario.AdministradorTiendaCry)]
    public async Task<ActionResult<DetallePromocionResponse>> ObtenerDetalle(int idTienda, int idPromocion)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            DetallePromocionResponse? response = await _promocionService.ObtenerDetalle(idTienda, idPromocion, idUsuario, baseUrl);

            if (response is null)
            {
                return NotFound("La promoción no existe.");
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

    [HttpGet("tienda/{idTienda:int}")]
    [Authorize(Roles = RolesUsuario.AdministradorTienda)]
    public async Task<ActionResult<List<PromocionListadoResponse>>> ObtenerPorTienda(int idTienda)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            List<PromocionListadoResponse> response = await _promocionService.ObtenerPorTienda(idTienda, idUsuario, baseUrl);

            return Ok(response);
        }
        catch (SinPermisoException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, e.Message);
        }
    }

    [HttpGet("tienda/{idTienda:int}/{idPromocion:int}/rechazo")]
    [Authorize(Roles = RolesUsuario.AdministradorTienda)]
    public async Task<ActionResult<RechazoPromocionResponse>> ObtenerRechazo(int idTienda, int idPromocion)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            RechazoPromocionResponse? response = await _promocionService.ObtenerRechazo(idTienda, idPromocion, idUsuario);

            if (response is null)
            {
                return NotFound("La promoción no existe o ya no está rechazada.");
            }

            return Ok(response);
        }
        catch (SinPermisoException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, e.Message);
        }
    }

    [HttpPut("tienda/{idTienda:int}/{idPromocion:int}/cancelar")]
    [Authorize(Roles = RolesUsuario.AdministradorTienda)]
    public async Task<IActionResult> Cancelar(int idTienda, int idPromocion)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            bool cancelada = await _promocionService.Cancelar(idTienda, idPromocion, idUsuario);

            if (!cancelada)
            {
                return NotFound("La promoción no existe.");
            }

            return NoContent();
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

    [HttpPost]
    [Authorize(Roles = RolesUsuario.AdministradorTienda)]
    public async Task<ActionResult<PromocionCreadaResponse>> SolicitarAlta(SolicitarAltaPromocionRequest request)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            PromocionCreadaResponse response = await _promocionService.SolicitarAlta(idUsuario, request);

            return StatusCode(StatusCodes.Status201Created, response);
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

    [HttpPost("{idPromocion:int}/imagen")]
    [Authorize(Roles = RolesUsuario.AdministradorTienda)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ImagenResponse>> SubirImagen(int idPromocion, [FromForm] IFormFile archivo)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            Imagen imagen = await _imagenService.SubirImagen(idPromocion, idUsuario, archivo);

            string url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{imagen.RutaRelativa}";

            ImagenResponse response = new ImagenResponse(imagen.Id, url);

            return Ok(response);
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
