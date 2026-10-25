using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaCry.Backend.Services;
using TiendaCry.Shared.DTO.Ciudad.Response;
using TiendaCry.Shared.DTO.Provincia.Response;

namespace TiendaCry.Backend.Controllers
{
    [ApiController]
    [Route("api/provincias")]
    [Authorize]
    public class ProvinciasController : ControllerBase
    {
        private readonly ProvinciaService _provinciaService;

        public ProvinciasController(ProvinciaService provinciaService)
        {
            _provinciaService = provinciaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProvinciaResponse>>> ObtenerProvincias()
        {
            List<ProvinciaResponse> response = await _provinciaService.ObtenerProvincias();

            return Ok(response);
        }

        [HttpGet("{idProvincia:int}/ciudades")]
        public async Task<ActionResult<List<CiudadResponse>>> ObtenerCiudades(int idProvincia)
        {
            List<CiudadResponse> response = await _provinciaService.ObtenerCiudadesPorProvincia(idProvincia);

            return Ok(response);
        }
    }
}
