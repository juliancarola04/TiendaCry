using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using TiendaCry.Shared.DTO.Imagen.Response;
using TiendaCry.Shared.DTO.Promocion.Request;
using TiendaCry.Shared.DTO.Promocion.Response;

namespace TiendaCry.Web.Services;

public class PromocionService
{
    private readonly AutenticacionService _autenticacionService;

    public PromocionService(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task<List<PromocionAdministradorResponse>> ObtenerAprobadas(int idTienda)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/administracion/promociones/tiendas/{idTienda}"));

        await ValidarRespuesta(respuesta, "No se pudieron obtener las promociones.");

        return await respuesta.Content.ReadFromJsonAsync<List<PromocionAdministradorResponse>>()
            ?? throw new InvalidOperationException("No se pudieron obtener las promociones.");
    }

    public async Task<List<PromocionAdministradorResponse>> ObtenerPendientes()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/administracion/promociones/pendientes"));

        await ValidarRespuesta(respuesta, "No se pudieron obtener las promociones pendientes.");

        return await respuesta.Content.ReadFromJsonAsync<List<PromocionAdministradorResponse>>()
            ?? throw new InvalidOperationException("No se pudieron obtener las promociones pendientes.");
    }

    public async Task Revisar(int idTienda, int idPromocion, RevisarPromocionRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, $"api/administracion/promociones/tiendas/{idTienda}/{idPromocion}/revision")
        {
            Content = JsonContent.Create(request)
        });

        await ValidarRespuesta(respuesta, "No se pudo revisar la promoción.");
    }

    public async Task CancelarComoAdministrador(int idTienda, int idPromocion)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, $"api/administracion/promociones/tiendas/{idTienda}/{idPromocion}/cancelar"));

        await ValidarRespuesta(respuesta, "No se pudo cancelar la promoción.");
    }

    public async Task<DetallePromocionResponse> ObtenerDetalle(int idTienda, int idPromocion)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/promociones/tienda/{idTienda}/{idPromocion}/detalles"));

        await ValidarRespuesta(respuesta, "No se pudieron obtener los detalles de la promoción.");

        return await respuesta.Content.ReadFromJsonAsync<DetallePromocionResponse>()
            ?? throw new InvalidOperationException("No se pudieron obtener los detalles de la promoción.");
    }

    public async Task<List<PromocionListadoResponse>> ObtenerPorTienda(int idTienda)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/promociones/tienda/{idTienda}"));

        await ValidarRespuesta(respuesta, "No se pudieron obtener las promociones.");

        return await respuesta.Content.ReadFromJsonAsync<List<PromocionListadoResponse>>()
            ?? throw new InvalidOperationException("No se pudieron obtener las promociones.");
    }

    public async Task<RechazoPromocionResponse> ObtenerRechazo(int idTienda, int idPromocion)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/promociones/tienda/{idTienda}/{idPromocion}/rechazo"));

        await ValidarRespuesta(respuesta, "No se pudo obtener el motivo del rechazo.");

        return await respuesta.Content.ReadFromJsonAsync<RechazoPromocionResponse>()
            ?? throw new InvalidOperationException("No se pudo obtener el motivo del rechazo.");
    }

    public async Task Cancelar(int idTienda, int idPromocion)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, $"api/promociones/tienda/{idTienda}/{idPromocion}/cancelar"));

        await ValidarRespuesta(respuesta, "No se pudo cancelar la promoción.");
    }

    public async Task<PromocionCreadaResponse> SolicitarAlta(SolicitarAltaPromocionRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, "api/promociones")
        {
            Content = JsonContent.Create(request)
        });

        await ValidarRespuesta(respuesta, "No se pudo crear la solicitud.");

        return await respuesta.Content.ReadFromJsonAsync<PromocionCreadaResponse>()
            ?? throw new InvalidOperationException("El servidor no devolvió la promoción creada.");
    }

    public async Task<ImagenResponse> SubirImagen(int idPromocion, IBrowserFile archivo)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => CrearSolicitudImagen(idPromocion, archivo));

        await ValidarRespuesta(respuesta, "No se pudo subir la imagen.");

        return await respuesta.Content.ReadFromJsonAsync<ImagenResponse>()
            ?? throw new InvalidOperationException("El servidor no devolvió los datos de la imagen.");
    }

    private HttpRequestMessage CrearSolicitudImagen(int idPromocion, IBrowserFile archivo)
    {
        MediaTypeHeaderValue tipoContenido = new MediaTypeHeaderValue(archivo.ContentType);

        MultipartFormDataContent contenido = new MultipartFormDataContent();
        StreamContent contenidoArchivo = new StreamContent(archivo.OpenReadStream(5 * 1024 * 1024));

        contenidoArchivo.Headers.ContentType = tipoContenido;
        contenido.Add(contenidoArchivo, "archivo", archivo.Name);

        return new HttpRequestMessage(HttpMethod.Post, $"api/promociones/{idPromocion}/imagen")
        {
            Content = contenido
        };
    }

    private async Task ValidarRespuesta(HttpResponseMessage respuesta, string mensajeError)
    {
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized
            || respuesta.StatusCode == HttpStatusCode.Forbidden
            || respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            respuesta.EnsureSuccessStatusCode();
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = mensajeError;
            }

            throw new InvalidOperationException(mensaje);
        }
    }
}
