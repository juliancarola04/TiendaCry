using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.DTO.Categoria.Response;

namespace TiendaCry.Backend.Services
{
    public class CategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<List<CategoriaResponse>> ObtenerCategorias()
        {
            List<Categoria> categorias = await _categoriaRepository.ObtenerCategorias();

            return categorias.Select(c => new CategoriaResponse(c.Id, c.Nombre)).ToList();
        }
    }
}
