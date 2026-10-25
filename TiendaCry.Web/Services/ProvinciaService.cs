using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Ciudad.Response;
using TiendaCry.Shared.DTO.Provincia.Response;

namespace TiendaCry.Web.Services;

public class ProvinciaService
{
    private readonly AutenticacionService _autenticacionService;

    public ProvinciaService(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task<List<ProvinciaResponse>> ObtenerProvincias()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/provincias"));
        respuesta.EnsureSuccessStatusCode();

        List<ProvinciaResponse>? provincias = await respuesta.Content.ReadFromJsonAsync<List<ProvinciaResponse>>();

        if (provincias is null)
        {
            throw new InvalidOperationException("No se pudieron obtener las provincias.");
        }

        return provincias;
    }

    public async Task<List<CiudadResponse>> ObtenerCiudadesPorProvincia(int idProvincia)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/provincias/{idProvincia}/ciudades"));
        respuesta.EnsureSuccessStatusCode();

        List<CiudadResponse>? ciudades = await respuesta.Content.ReadFromJsonAsync<List<CiudadResponse>>();

        if (ciudades is null)
        {
            throw new InvalidOperationException("No se pudieron obtener las ciudades.");
        }

        return ciudades;
    }
}
