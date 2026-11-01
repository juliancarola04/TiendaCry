using Microsoft.IdentityModel.JsonWebTokens;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Mobile.Services;

public class AutenticacionService
{
    private readonly HttpClient _httpClient;
    private readonly AlmacenamientoTokensService _almacenamientoTokensService;

    public event Action? SesionCambio;

    public AutenticacionService(IHttpClientFactory httpClientFactory, AlmacenamientoTokensService almacenamientoTokensService)
    {
        _httpClient = httpClientFactory.CreateClient("TiendaCry");
        _almacenamientoTokensService = almacenamientoTokensService;
    }

    public async Task<string?> ObtenerTokenVigente()
    {
        AutenticacionResponse? sesion = await _almacenamientoTokensService.Obtener();

        if (sesion is null)
        {
            return null;
        }

        JsonWebTokenHandler handler = new JsonWebTokenHandler();

        if (string.IsNullOrWhiteSpace(sesion.Token) || !handler.CanReadToken(sesion.Token))
        {
            LimpiarSesion();

            return null;
        }

        JsonWebToken jwt = handler.ReadJsonWebToken(sesion.Token);

        if (jwt.ValidTo <= DateTime.UtcNow)
        {
            await RenovarSesion();

            sesion = await _almacenamientoTokensService.Obtener();
        }

        return sesion?.Token;
    }

    public async Task EnviarVerificacionEmail()
    {
        using HttpResponseMessage respuesta = await EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, "api/auth/enviar-verificacion-email"));

        await ValidarRespuesta(respuesta, "No se pudo enviar el correo de verificación.");
    }

    public async Task CambiarContrasenaTemporal(CambiarContrasenaTemporalRequest request)
    {
        using HttpResponseMessage respuesta = await EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, "api/auth/cambiar-contrasena-temporal")
        {
            Content = JsonContent.Create(request)
        });

        if (respuesta.StatusCode == HttpStatusCode.Forbidden)
        {
            respuesta.EnsureSuccessStatusCode();
        }

        await ValidarRespuesta(respuesta, "No se pudo cambiar la contraseña temporal.");
    }

    public async Task ModificarEmail(ModificarEmailRequest request)
    {
        using HttpResponseMessage respuesta = await EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, "api/auth/email-sin-verificar")
        {
            Content = JsonContent.Create(request)
        });

        await ValidarRespuesta(respuesta, "No se pudo modificar el correo.");
    }

    public async Task Registrarse(RegisterRequest request)
    {
        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/register", request);

        await ValidarRespuesta(respuesta, "No se pudo crear la cuenta.");

        AutenticacionResponse? registro = await respuesta.Content.ReadFromJsonAsync<AutenticacionResponse>();

        if (registro is null || string.IsNullOrWhiteSpace(registro.Token) || string.IsNullOrWhiteSpace(registro.RefreshToken))
        {
            throw new InvalidOperationException("El servidor no devolvió una sesión válida.");
        }

        await GuardarSesion(registro);
    }

    public async Task SolicitarRecuperacionContrasena(SolicitarRecuperacionContrasenaRequest request)
    {
        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/solicitar-recuperacion-contrasena", request);

        await ValidarRespuesta(respuesta, "No se pudo solicitar la recuperación de contraseña.");
    }

    public async Task CerrarSesion()
    {
        AutenticacionResponse? sesion = await _almacenamientoTokensService.Obtener();

        if (sesion is not null)
        {
            LogoutRequest request = new LogoutRequest(sesion.RefreshToken);

            using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/logout", request);

            await ValidarRespuesta(respuesta, "No se pudo cerrar la sesión.");
        }

        LimpiarSesion();
    }

    public async Task IniciarSesion(LoginRequest request)
    {
        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        await ValidarRespuesta(respuesta, "No se pudo iniciar sesión.");

        AutenticacionResponse? sesion = await respuesta.Content.ReadFromJsonAsync<AutenticacionResponse>();

        if (sesion is null || string.IsNullOrWhiteSpace(sesion.Token) || string.IsNullOrWhiteSpace(sesion.RefreshToken))
        {
            throw new InvalidOperationException("El servidor no devolvió una sesión válida.");
        }

        await GuardarSesion(sesion);
    }

    public async Task<HttpResponseMessage> EnviarAutenticado(Func<HttpRequestMessage> crearSolicitud)
    {
        HttpResponseMessage respuesta = await EnviarConToken(crearSolicitud);

        if (respuesta.StatusCode != HttpStatusCode.Unauthorized)
        {
            return respuesta;
        }

        respuesta.Dispose();

        await RenovarSesion();

        respuesta = await EnviarConToken(crearSolicitud);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
        {
            LimpiarSesion();
            respuesta.Dispose();

            throw new HttpRequestException("La sesión ya no es válida.", null, HttpStatusCode.Unauthorized);
        }

        return respuesta;
    }

    private async Task<HttpResponseMessage> EnviarConToken(Func<HttpRequestMessage> crearSolicitud)
    {
        AutenticacionResponse? sesion = await _almacenamientoTokensService.Obtener();

        if (sesion is null || string.IsNullOrWhiteSpace(sesion.Token) || string.IsNullOrWhiteSpace(sesion.RefreshToken))
        {
            LimpiarSesion();

            throw new HttpRequestException("No hay una sesión válida guardada.", null, HttpStatusCode.Unauthorized);
        }

        using HttpRequestMessage solicitud = crearSolicitud();
        solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sesion.Token);

        return await _httpClient.SendAsync(solicitud);
    }

    public async Task RenovarSesion()
    {
        AutenticacionResponse? sesion = await _almacenamientoTokensService.Obtener();

        if (sesion is null || string.IsNullOrWhiteSpace(sesion.RefreshToken))
        {
            LimpiarSesion();

            throw new HttpRequestException("No hay una sesión válida guardada.", null, HttpStatusCode.Unauthorized);
        }

        RefreshRequest request = new RefreshRequest(sesion.RefreshToken);

        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
        {
            LimpiarSesion();
        }

        respuesta.EnsureSuccessStatusCode();

        AutenticacionResponse? nuevaSesion = await respuesta.Content.ReadFromJsonAsync<AutenticacionResponse>();

        if (nuevaSesion is null || string.IsNullOrWhiteSpace(nuevaSesion.Token) || string.IsNullOrWhiteSpace(nuevaSesion.RefreshToken))
        {
            throw new InvalidOperationException("El servidor no devolvió una sesión válida.");
        }

        await GuardarSesion(nuevaSesion);
    }

    private async Task GuardarSesion(AutenticacionResponse sesion)
    {
        await _almacenamientoTokensService.Guardar(sesion);

        SesionCambio?.Invoke();
    }

    private void LimpiarSesion()
    {
        _almacenamientoTokensService.Limpiar();

        SesionCambio?.Invoke();
    }

    private async Task ValidarRespuesta(HttpResponseMessage respuesta, string mensajeError)
    {
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
