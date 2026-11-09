using System.Net;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Socio.Request;
using TiendaCry.Shared.DTO.Socio.Response;

namespace TiendaCry.Mobile.Services;

public class SocioService
{
    private readonly AutenticacionService _autenticacionService;

    public SocioService(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task CrearSocio()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, "api/socio/alta"));

        await ComprobarRespuesta(respuesta);
    }

    public async Task<UbicacionSocioResponse> ObtenerUbicacion()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/socio/ubicacion"));

        await ComprobarRespuesta(respuesta);

        return await respuesta.Content.ReadFromJsonAsync<UbicacionSocioResponse>()
            ?? throw new InvalidOperationException("No se pudo obtener tu ubicación.");
    }

    public async Task GuardarUbicacion(GuardarUbicacionRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, "api/socio/ubicacion")
        {
            Content = JsonContent.Create(request)
        });

        await ComprobarRespuesta(respuesta);
    }

    public async Task<List<PromocionSocioResponse>> ObtenerPromociones(bool cerca)
    {
        string ruta = "api/socio/promociones";

        if (cerca)
        {
            ruta += "?cerca=true";
        }

        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, ruta));

        await ComprobarRespuesta(respuesta);

        return await respuesta.Content.ReadFromJsonAsync<List<PromocionSocioResponse>>()
            ?? throw new InvalidOperationException("No se pudieron obtener las promociones.");
    }

    public async Task<DetallePromocionSocioResponse> ObtenerDetalle(int idPromocion)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/socio/promociones/{idPromocion}"));

        await ComprobarRespuesta(respuesta);

        return await respuesta.Content.ReadFromJsonAsync<DetallePromocionSocioResponse>()
            ?? throw new InvalidOperationException("No se pudo obtener la promoción.");
    }

    private async Task ComprobarRespuesta(HttpResponseMessage respuesta)
    {
        if (respuesta.StatusCode == HttpStatusCode.BadRequest || respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = "No se pudo completar la solicitud.";
            }

            throw new InvalidOperationException(mensaje);
        }

        respuesta.EnsureSuccessStatusCode();
    }
}
