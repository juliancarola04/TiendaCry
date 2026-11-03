using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.Auth;
using TiendaCry.Shared.DTO.Promocion.Response;
using TiendaCry.Shared.DTO.Tienda.Response;

namespace TiendaCry.Backend.Controllers;

[ApiController]
[Route("api/empleado/tiendas")]
[Authorize(Roles = RolesUsuario.EmpleadoTienda)]
public class TiendasEmpleadoController : ControllerBase
{
    private readonly TiendaService _tiendaService;
    private readonly PromocionService _promocionService;

    public TiendasEmpleadoController(TiendaService tiendaService, PromocionService promocionService)
    {
        _tiendaService = tiendaService;
        _promocionService = promocionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TiendaActivaResponse>>> ObtenerTiendas()
    {
        int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);

        List<TiendaActivaResponse> response = await _tiendaService.ObtenerTiendasEmpleado(idUsuario);

        return Ok(response);
    }

    [HttpGet("{idTienda:int}/promociones")]
    public async Task<ActionResult<PromocionesTiendaEmpleadoResponse>> ObtenerPromociones(int idTienda)
    {
        try
        {
            int idUsuario = Convert.ToInt32(User.FindFirst("sub")?.Value);
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            PromocionesTiendaEmpleadoResponse? response = await _promocionService.ObtenerParaEmpleado(idTienda, idUsuario, baseUrl);

            if (response is null)
            {
                return NotFound("La tienda no existe o ya no está aprobada.");
            }

            return Ok(response);
        }
        catch (SinPermisoException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, e.Message);
        }
    }
}
