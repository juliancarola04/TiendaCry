using System.Net;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Categoria.Request;
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

    public async Task Crear(GuardarCategoriaRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, "api/categorias")
        {
            Content = JsonContent.Create(request)
        });

        await ComprobarRespuesta(respuesta);
    }

    public async Task Modificar(int idCategoria, GuardarCategoriaRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, $"api/categorias/{idCategoria}")
        {
            Content = JsonContent.Create(request)
        });

        await ComprobarRespuesta(respuesta);
    }

    public async Task Eliminar(int idCategoria)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Delete, $"api/categorias/{idCategoria}"));

        await ComprobarRespuesta(respuesta);
    }

    private async Task ComprobarRespuesta(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode)
        {
            return;
        }

        if (respuesta.StatusCode == HttpStatusCode.BadRequest
            || respuesta.StatusCode == HttpStatusCode.Conflict
            || respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = "No se pudo completar la operación.";
            }

            throw new InvalidOperationException(mensaje);
        }

        respuesta.EnsureSuccessStatusCode();
    }
}
