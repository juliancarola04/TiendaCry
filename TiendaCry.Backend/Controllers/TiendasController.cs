using TiendaCry.Shared.DTO.Tienda.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.DTO.Tienda.Response;

namespace TiendaCry.Backend.Controllers
{
    [ApiController]
    [Route("api/tiendas")]
    [Authorize]
    public class TiendasController : ControllerBase
    {
        private readonly TiendaService _tiendaService;

        public TiendasController(TiendaService tiendaService)
        {
            _tiendaService = tiendaService;
        }

        [HttpGet("activa")]
        public async Task<ActionResult<TiendaActivaResponse>> ObtenerTiendaActiva()
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                TiendaActivaResponse? response = await _tiendaService.ObtenerTiendaActiva(idUsuario);

                if (response is null)
                {
                    return NoContent();
                }

                return Ok(response);
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
            }
            catch (SinPermisoException e)
            {
                return StatusCode(StatusCodes.Status403Forbidden, e.Message);
            }
        }

        [HttpGet("solicitudes")]
        public async Task<ActionResult<List<SolicitudTiendaResponse>>> ObtenerSolicitudes()
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                List<SolicitudTiendaResponse> response = await _tiendaService.ObtenerSolicitudes(idUsuario);

                return Ok(response);
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
            }
            catch (SinPermisoException e)
            {
                return StatusCode(StatusCodes.Status403Forbidden, e.Message);
            }
        }
        [HttpPost("solicitudes")]
        public async Task<ActionResult<SolicitudTiendaResponse>> SolicitarAlta(SolicitarAltaTiendaRequest request)
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                SolicitudTiendaResponse response = await _tiendaService.SolicitarAlta(idUsuario, request);

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
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

        [HttpGet("solicitudes/{idTienda:int}")]
        public async Task<ActionResult<DetalleSolicitudTiendaResponse>> ObtenerSolicitudRechazada(int idTienda)
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                DetalleSolicitudTiendaResponse? response = await _tiendaService.ObtenerSolicitudRechazada(idTienda, idUsuario);

                if (response is null)
                {
                    return NotFound("No se encontró una solicitud rechazada disponible para este usuario.");
                }

                return Ok(response);
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
            }
            catch (SinPermisoException e)
            {
                return StatusCode(StatusCodes.Status403Forbidden, e.Message);
            }
        }

        [HttpPut("solicitudes/{idTienda:int}")]
        public async Task<ActionResult<SolicitudTiendaResponse>> ReenviarSolicitud(int idTienda, ReenviarSolicitudTiendaRequest request)
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                SolicitudTiendaResponse? response = await _tiendaService.ReenviarSolicitud(idTienda, idUsuario, request);

                if (response is null)
                {
                    return NotFound("No se encontró una solicitud rechazada disponible para este usuario.");
                }

                return Ok(response);
            }
            catch (CredencialesInvalidasException e)
            {
                return Unauthorized(e.Message);
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
}
