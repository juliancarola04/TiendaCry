using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.Auth;
using TiendaCry.Shared.DTO.Empleado.Request;
using TiendaCry.Shared.DTO.Empleado.Response;

namespace TiendaCry.Backend.Controllers;

[ApiController]
[Route("api/tiendas/{idTienda:int}/empleados")]
[Authorize(Roles = RolesUsuario.AdministradorTienda)]
public class EmpleadosController : ControllerBase
{
    private readonly EmpleadoService _empleadoService;

    public EmpleadosController(EmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmpleadoResponse>>> ObtenerEmpleados(int idTienda)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            List<EmpleadoResponse> response = await _empleadoService.ObtenerEmpleados(idTienda, idUsuario);

            return Ok(response);
        }
        catch (SinPermisoException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, e.Message);
        }
    }

    [HttpPost("consultar-correo")]
    public async Task<ActionResult<ConsultarCorreoEmpleadoResponse>> ConsultarCorreo(int idTienda, ConsultarCorreoEmpleadoRequest request)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            ConsultarCorreoEmpleadoResponse response = await _empleadoService.ConsultarCorreo(idTienda, idUsuario, request);

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
        catch (RecursoExistenteException e)
        {
            return Conflict(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AgregarEmpleado(int idTienda, AgregarEmpleadoRequest request)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            await _empleadoService.AgregarEmpleado(idTienda, idUsuario, request);

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
        catch (RecursoExistenteException e)
        {
            return Conflict(e.Message);
        }
    }

    [HttpPut("{idUsuarioTienda:int}/baja")]
    public async Task<IActionResult> DarDeBaja(int idTienda, int idUsuarioTienda)
    {
        try
        {
            string? sub = User.FindFirst("sub")?.Value;
            int idUsuario = Convert.ToInt32(sub);

            bool dadaDeBaja = await _empleadoService.DarDeBaja(idTienda, idUsuarioTienda, idUsuario);

            if (!dadaDeBaja)
            {
                return NotFound("No se encontró un empleado activo en esta tienda.");
            }

            return NoContent();
        }
        catch (SinPermisoException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, e.Message);
        }
    }

}
