using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.DTO.Ciudad.Response;
using TiendaCry.Shared.DTO.Provincia.Response;

namespace TiendaCry.Backend.Services
{
    public class ProvinciaService
    {
        private readonly IProvinciaRepository _provinciaRepository;

        public ProvinciaService(IProvinciaRepository provinciaRepository)
        {
            _provinciaRepository = provinciaRepository;
        }

        public async Task<List<ProvinciaResponse>> ObtenerProvincias()
        {
            List<Provincia> provincias = await _provinciaRepository.ObtenerProvincias();

            return provincias.Select(p => new ProvinciaResponse(p.Id, p.Nombre)).ToList();
        }

        public async Task<List<CiudadResponse>> ObtenerCiudadesPorProvincia(int idProvincia)
        {
            List<Ciudad> ciudades = await _provinciaRepository.ObtenerCiudadesPorProvincia(idProvincia);

            return ciudades.Select(c => new CiudadResponse(c.Id, c.Nombre)).ToList();
        }
    }
}
