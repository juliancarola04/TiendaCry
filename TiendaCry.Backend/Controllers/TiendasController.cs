using TiendaCry.Shared.DTO.Tienda.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.DTO.Tienda.Response;
using TiendaCry.Shared.Auth;

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

        [HttpPut("{idTienda:int}/baja")]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<IActionResult> DarDeBaja(int idTienda)
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                bool dadaDeBaja = await _tiendaService.DarDeBaja(idTienda, idUsuario);

                if (!dadaDeBaja)
                {
                    return NotFound("La tienda no existe o ya no está aprobada.");
                }

                return NoContent();
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

        [HttpGet("aprobadas")]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<ActionResult<List<TiendaActivaResponse>>> ObtenerTiendasAprobadas()
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                List<TiendaActivaResponse> response = await _tiendaService.ObtenerTiendasAprobadas(idUsuario);

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

        [HttpGet("aprobadas/{idTienda:int}")]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<ActionResult<DetalleTiendaResponse>> ObtenerDetalleTiendaAprobada(int idTienda)
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                DetalleTiendaResponse? response = await _tiendaService.ObtenerDetalleTiendaAprobada(idTienda, idUsuario);

                if (response is null)
                {
                    return NotFound("La tienda no existe o ya no está aprobada.");
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

        [HttpGet("solicitudes/pendientes")]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<ActionResult<List<SolicitudTiendaPendienteResponse>>> ObtenerSolicitudesPendientes()
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                List<SolicitudTiendaPendienteResponse> response = await _tiendaService.ObtenerSolicitudesPendientes(idUsuario);

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

        [HttpGet("solicitudes/pendientes/{idTienda:int}")]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<ActionResult<DetalleTiendaResponse>> ObtenerDetalleSolicitudPendiente(int idTienda)
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                DetalleTiendaResponse? response = await _tiendaService.ObtenerDetalleSolicitudPendiente(idTienda, idUsuario);

                if (response is null)
                {
                    return NotFound("La solicitud no existe o ya no está pendiente.");
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

        [HttpPut("solicitudes/pendientes/{idTienda:int}/aceptar")]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<IActionResult> AceptarSolicitud(int idTienda)
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                bool resuelta = await _tiendaService.AceptarSolicitud(idTienda, idUsuario);

                if (!resuelta)
                {
                    return NotFound("La solicitud no existe o ya no está pendiente.");
                }

                return NoContent();
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

        [HttpPut("solicitudes/pendientes/{idTienda:int}/rechazar")]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<IActionResult> RechazarSolicitud(int idTienda, RechazarSolicitudTiendaRequest request)
        {
            try
            {
                string? sub = User.FindFirst("sub")?.Value;
                int idUsuario = Convert.ToInt32(sub);

                bool resuelta = await _tiendaService.RechazarSolicitud(idTienda, idUsuario, request);

                if (!resuelta)
                {
                    return NotFound("La solicitud no existe o ya no está pendiente.");
                }

                return NoContent();
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
