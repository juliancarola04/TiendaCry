using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Services;
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
    }
}
