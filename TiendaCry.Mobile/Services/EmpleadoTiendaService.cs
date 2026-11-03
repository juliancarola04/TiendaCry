using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Promocion.Response;
using TiendaCry.Shared.DTO.Tienda.Response;

namespace TiendaCry.Mobile.Services;

public class EmpleadoTiendaService
{
    private readonly AutenticacionService _autenticacionService;

    public EmpleadoTiendaService(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task<List<TiendaActivaResponse>> ObtenerTiendas()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/empleado/tiendas"));

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<List<TiendaActivaResponse>>()
            ?? throw new InvalidOperationException("No se pudieron obtener las tiendas.");
    }

    public async Task<PromocionEmpleadoResponse> ObtenerPromocion(int idTienda, int idPromocion)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/empleado/tiendas/{idTienda}/promociones/{idPromocion}"));

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<PromocionEmpleadoResponse>()
            ?? throw new InvalidOperationException("No se pudo obtener la promoción.");
    }

    public async Task<PromocionesTiendaEmpleadoResponse> ObtenerPromociones(int idTienda)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/empleado/tiendas/{idTienda}/promociones"));

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<PromocionesTiendaEmpleadoResponse>()
            ?? throw new InvalidOperationException("No se pudieron obtener las promociones.");
    }
}
