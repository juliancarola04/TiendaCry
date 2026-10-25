using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Categoria.Response;

namespace TiendaCry.Web.Services;

public class CategoriaService
{
    private readonly AutenticacionService _autenticacionService;

    public CategoriaService(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task<List<CategoriaResponse>> ObtenerCategorias()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/categorias"));
        respuesta.EnsureSuccessStatusCode();

        List<CategoriaResponse>? categorias = await respuesta.Content.ReadFromJsonAsync<List<CategoriaResponse>>();

        if (categorias is null)
        {
            throw new InvalidOperationException("No se pudieron obtener las categorías.");
        }

        return categorias;
    }
}
