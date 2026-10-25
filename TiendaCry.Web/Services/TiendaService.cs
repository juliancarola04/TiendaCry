using System.Net;
using TiendaCry.Shared.DTO.Tienda.Request;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Tienda.Response;

namespace TiendaCry.Web.Services;

public class TiendaService
{
    private readonly AutenticacionService _autenticacionService;

    public TiendaService(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task<TiendaActivaResponse?> ObtenerTiendaActiva()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/tiendas/activa"));
        respuesta.EnsureSuccessStatusCode();

        if (respuesta.StatusCode == HttpStatusCode.NoContent)
        {
            return null;
        }

        TiendaActivaResponse? tienda = await respuesta.Content.ReadFromJsonAsync<TiendaActivaResponse>();

        if (tienda is null)
        {
            throw new InvalidOperationException("No se pudo obtener la tienda activa.");
        }

        return tienda;
    }

    public async Task<List<SolicitudTiendaResponse>> ObtenerSolicitudes()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/tiendas/solicitudes"));
        respuesta.EnsureSuccessStatusCode();

        List<SolicitudTiendaResponse>? solicitudes = await respuesta.Content.ReadFromJsonAsync<List<SolicitudTiendaResponse>>();

        if (solicitudes is null)
        {
            throw new InvalidOperationException("No se pudieron obtener las solicitudes de tiendas.");
        }

        return solicitudes;
    }

    public async Task SolicitarAlta(SolicitarAltaTiendaRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, "api/tiendas/solicitudes")
        {
            Content = JsonContent.Create(request)
        });

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
        {
            respuesta.EnsureSuccessStatusCode();
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = "No se pudo enviar la solicitud de alta de la tienda.";
            }

            throw new InvalidOperationException(mensaje);
        }
    }

    public async Task<DetalleSolicitudTiendaResponse> ObtenerSolicitudRechazada(int idTienda)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/tiendas/solicitudes/{idTienda}"));
        respuesta.EnsureSuccessStatusCode();

        DetalleSolicitudTiendaResponse? solicitud = await respuesta.Content.ReadFromJsonAsync<DetalleSolicitudTiendaResponse>();

        if (solicitud is null)
        {
            throw new InvalidOperationException("No se pudieron obtener los datos de la solicitud.");
        }

        return solicitud;
    }

    public async Task ReenviarSolicitud(int idTienda, ReenviarSolicitudTiendaRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, $"api/tiendas/solicitudes/{idTienda}")
        {
            Content = JsonContent.Create(request)
        });

        if (respuesta.StatusCode == HttpStatusCode.Forbidden || respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            respuesta.EnsureSuccessStatusCode();
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = "No se pudo reenviar la solicitud de la tienda.";
            }

            throw new InvalidOperationException(mensaje);
        }
    }
}
