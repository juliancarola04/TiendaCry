using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Web.Services;

public class AutenticacionService
{
    private readonly HttpClient _httpClient;

    public AutenticacionService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("TiendaCry");
    }

    public async Task IniciarSesion(LoginRequest request)
    {
        await EnviarConCsrf(HttpMethod.Post, "api/auth/web/login", JsonContent.Create(request), "No se pudo iniciar sesión.");
    }

    public async Task Registrarse(RegisterRequest request)
    {
        await EnviarConCsrf(HttpMethod.Post, "api/auth/web/register", JsonContent.Create(request), "No se pudo crear la cuenta.");
    }

    public async Task<UsuarioActualResponse> ObtenerUsuarioActual()
    {
        using HttpRequestMessage solicitud = new HttpRequestMessage(HttpMethod.Get, "api/auth/usuario-actual");
        solicitud.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        using HttpResponseMessage respuesta = await _httpClient.SendAsync(solicitud);
        respuesta.EnsureSuccessStatusCode();

        UsuarioActualResponse? usuario = await respuesta.Content.ReadFromJsonAsync<UsuarioActualResponse>();

        if (usuario is null)
        {
            throw new InvalidOperationException("No se pudieron obtener los datos del usuario.");
        }

        return usuario;
    }

    public async Task EnviarVerificacionEmail()
    {
        await EnviarConCsrf(HttpMethod.Post, "api/auth/enviar-verificacion-email", null, "No se pudo enviar el correo de verificación.", true);
    }

    public async Task ConfirmarEmail(string token)
    {
        ConfirmarEmailRequest request = new ConfirmarEmailRequest(token);

        await EnviarConCsrf(HttpMethod.Post, "api/auth/confirmar-email", JsonContent.Create(request), "No se pudo confirmar el correo.");
    }

    public async Task ModificarEmail(ModificarEmailRequest request)
    {
        await EnviarConCsrf(HttpMethod.Put, "api/auth/email-sin-verificar", JsonContent.Create(request), "No se pudo modificar el correo.", true);
    }

    public async Task CerrarSesion()
    {
        await EnviarConCsrf(HttpMethod.Post, "api/auth/web/logout", null, "No se pudo cerrar la sesión.");
    }

    private async Task EnviarConCsrf(
        HttpMethod metodo, string ruta, HttpContent? contenido, string mensajeError, bool requiereSesion = false)
    {
        using HttpRequestMessage solicitud = new HttpRequestMessage(metodo, ruta);
        solicitud.Content = contenido;

        string tokenCsrf = await ObtenerTokenCsrf();

        solicitud.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        solicitud.Headers.Add("X-CSRF-TOKEN", tokenCsrf);

        using HttpResponseMessage respuesta = await _httpClient.SendAsync(solicitud);

        if (requiereSesion && respuesta.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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

    private async Task<string> ObtenerTokenCsrf()
    {
        using HttpRequestMessage solicitudCsrf = new HttpRequestMessage(HttpMethod.Get, "api/auth/web/csrf");
        solicitudCsrf.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        // Acá sería donde volvería el JSON que desp lo vamos a usar
        using HttpResponseMessage respuestaCsrf = await _httpClient.SendAsync(solicitudCsrf);
        respuestaCsrf.EnsureSuccessStatusCode();

        CsrfResponse? csrf = await respuestaCsrf.Content.ReadFromJsonAsync<CsrfResponse>();

        if (csrf is null || string.IsNullOrWhiteSpace(csrf.Token))
        {
            throw new InvalidOperationException("Falló el generar el token CSRF.");
        }

        return csrf.Token;
    }

    private record CsrfResponse(string Token);
}
