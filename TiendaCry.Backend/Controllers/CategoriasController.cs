using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.Auth;
using TiendaCry.Shared.DTO.Categoria.Request;
using TiendaCry.Shared.DTO.Categoria.Response;

namespace TiendaCry.Backend.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly CategoriaService _categoriaService;

        public CategoriasController(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaResponse>>> ObtenerCategorias()
        {
            List<CategoriaResponse> response = await _categoriaService.ObtenerCategorias();

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<IActionResult> Crear(GuardarCategoriaRequest request)
        {
            try
            {
                int idAdministrador = Convert.ToInt32(User.FindFirst("sub")?.Value);

                await _categoriaService.Crear(idAdministrador, request);

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

        [HttpPut("{idCategoria:int}")]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<IActionResult> Modificar(int idCategoria, GuardarCategoriaRequest request)
        {
            try
            {
                int idAdministrador = Convert.ToInt32(User.FindFirst("sub")?.Value);

                bool modificada = await _categoriaService.Modificar(idCategoria, idAdministrador, request);

                if (!modificada)
                {
                    return NotFound("La categoría no existe.");
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
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpDelete("{idCategoria:int}")]
        [Authorize(Roles = RolesUsuario.AdministradorTiendaCry)]
        public async Task<IActionResult> Eliminar(int idCategoria)
        {
            try
            {
                int idAdministrador = Convert.ToInt32(User.FindFirst("sub")?.Value);

                bool eliminada = await _categoriaService.Eliminar(idCategoria, idAdministrador);

                if (!eliminada)
                {
                    return NotFound("La categoría no existe.");
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
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
        }
    }
}
