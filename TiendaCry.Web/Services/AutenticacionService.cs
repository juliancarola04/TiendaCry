using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Web.Services;

public class AutenticacionService
{
    private readonly HttpClient _httpClient;
    private readonly AlmacenamientoTokensService _almacenamientoTokensService;

    public AutenticacionService(IHttpClientFactory httpClientFactory, AlmacenamientoTokensService almacenamientoTokensService)
    {
        _httpClient = httpClientFactory.CreateClient("TiendaCry");
        _almacenamientoTokensService = almacenamientoTokensService;
    }

    public async Task<UsuarioActualResponse> ObtenerUsuarioActual()
    {
        using HttpResponseMessage respuesta = await EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/auth/usuario-actual"));
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
        using HttpResponseMessage respuesta = await EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, "api/auth/enviar-verificacion-email"));

        await ValidarRespuesta(respuesta, "No se pudo enviar el correo de verificación.");
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

        await _almacenamientoTokensService.Guardar(registro);
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

        await _almacenamientoTokensService.Limpiar();
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

        await _almacenamientoTokensService.Guardar(sesion);
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
            await _almacenamientoTokensService.Limpiar();
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
            await _almacenamientoTokensService.Limpiar();

            throw new HttpRequestException("No hay una sesión válida guardada.", null, HttpStatusCode.Unauthorized);
        }

        using HttpRequestMessage solicitud = crearSolicitud();
        solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sesion.Token);

        return await _httpClient.SendAsync(solicitud);
    }

    private async Task RenovarSesion()
    {
        AutenticacionResponse? sesion = await _almacenamientoTokensService.Obtener();

        if (sesion is null || string.IsNullOrWhiteSpace(sesion.RefreshToken))
        {
            await _almacenamientoTokensService.Limpiar();

            throw new HttpRequestException("No hay una sesión válida guardada.", null, HttpStatusCode.Unauthorized);
        }

        RefreshRequest request = new RefreshRequest(sesion.RefreshToken);

        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _almacenamientoTokensService.Limpiar();
        }

        respuesta.EnsureSuccessStatusCode();

        AutenticacionResponse? nuevaSesion = await respuesta.Content.ReadFromJsonAsync<AutenticacionResponse>();

        if (nuevaSesion is null || string.IsNullOrWhiteSpace(nuevaSesion.Token) || string.IsNullOrWhiteSpace(nuevaSesion.RefreshToken))
        {
            throw new InvalidOperationException("El servidor no devolvió una sesión válida.");
        }

        await _almacenamientoTokensService.Guardar(nuevaSesion);
    }

    public async Task ConfirmarEmail(string token)
    {
        ConfirmarEmailRequest request = new ConfirmarEmailRequest(token);

        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/confirmar-email", request);

        await ValidarRespuesta(respuesta, "No se pudo confirmar el correo.");
    }

    public async Task RestablecerContrasena(RestablecerContrasenaRequest request)
    {
        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/restablecer-contrasena", request);

        await ValidarRespuesta(respuesta, "No se pudo restablecer la contraseña.");
    }

    // Con este me simplifico un montón el tema de los mensajes de error. Recordar!!
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
