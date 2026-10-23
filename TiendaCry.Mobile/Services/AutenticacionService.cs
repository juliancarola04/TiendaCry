using System.Net.Http.Headers;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Mobile.Services;

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

        await ComprobarRespuesta(respuesta, "No se pudo enviar el correo de verificación.");
    }

    public async Task ModificarEmail(ModificarEmailRequest request)
    {
        using HttpResponseMessage respuesta = await EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, "api/auth/email-sin-verificar")
        {
            Content = JsonContent.Create(request)
        });

        await ComprobarRespuesta(respuesta, "No se pudo modificar el correo.");
    }

    private async Task RenovarSesion()
    {
        LoginResponse? sesion = await _almacenamientoTokensService.Obtener();

        if (sesion is null)
        {
            throw new HttpRequestException("No hay una sesión guardada.", null, System.Net.HttpStatusCode.Unauthorized);
        }

        RefreshRequest request = new RefreshRequest(sesion.RefreshToken);

        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);
        respuesta.EnsureSuccessStatusCode();

        LoginResponse? nuevaSesion = await respuesta.Content.ReadFromJsonAsync<LoginResponse>();

        if (nuevaSesion is null || string.IsNullOrWhiteSpace(nuevaSesion.Token) || string.IsNullOrWhiteSpace(nuevaSesion.RefreshToken))
        {
            throw new InvalidOperationException("El servidor no devolvió una sesión válida.");
        }

        await _almacenamientoTokensService.Guardar(nuevaSesion);
    }

    private async Task<HttpResponseMessage> EnviarConToken(Func<HttpRequestMessage> crearSolicitud)
    {
        LoginResponse? sesion = await _almacenamientoTokensService.Obtener();

        if (sesion is null)
        {
            throw new HttpRequestException("No hay una sesión guardada.", null, System.Net.HttpStatusCode.Unauthorized);
        }

        using HttpRequestMessage solicitud = crearSolicitud();
        solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sesion.Token);

        return await _httpClient.SendAsync(solicitud);
    }

    private async Task<HttpResponseMessage> EnviarAutenticado(Func<HttpRequestMessage> crearSolicitud)
    {
        HttpResponseMessage respuesta = await EnviarConToken(crearSolicitud);

        if (respuesta.StatusCode != System.Net.HttpStatusCode.Unauthorized)
        {
            return respuesta;
        }

        respuesta.Dispose();

        await RenovarSesion();

        return await EnviarConToken(crearSolicitud);
    }

    private async Task ComprobarRespuesta(HttpResponseMessage respuesta, string mensajeError)
    {
        if (respuesta.StatusCode == System.Net.HttpStatusCode.Unauthorized)
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

    public async Task Registrarse(RegisterRequest request)
    {
        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/register", request);

        if (!respuesta.IsSuccessStatusCode)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = "No se pudo crear la cuenta.";
            }

            throw new InvalidOperationException(mensaje);
        }

        RegisterResponse? registro = await respuesta.Content.ReadFromJsonAsync<RegisterResponse>();

        if (registro is null || string.IsNullOrWhiteSpace(registro.Token) || string.IsNullOrWhiteSpace(registro.RefreshToken))
        {
            throw new InvalidOperationException("El servidor no devolvió una sesión válida.");
        }

        LoginResponse sesion = new LoginResponse(registro.Token, registro.RefreshToken, registro.SesionExpira);

        await _almacenamientoTokensService.Guardar(sesion);
    }

    public async Task SolicitarRecuperacionContrasena(SolicitarRecuperacionContrasenaRequest request)
    {
        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/solicitar-recuperacion-contrasena", request);

        if (!respuesta.IsSuccessStatusCode)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = "No se pudo solicitar la recuperación de contraseña.";
            }

            throw new InvalidOperationException(mensaje);
        }
    }

    public async Task CerrarSesion()
    {
        LoginResponse? sesion = await _almacenamientoTokensService.Obtener();

        if (sesion is not null)
        {
            LogoutRequest request = new LogoutRequest(sesion.RefreshToken);

            using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/logout", request);

            if (!respuesta.IsSuccessStatusCode)
            {
                string mensaje = await respuesta.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(mensaje))
                {
                    mensaje = "No se pudo cerrar la sesión.";
                }

                throw new InvalidOperationException(mensaje);
            }
        }

        _almacenamientoTokensService.Limpiar();
    }

    public async Task IniciarSesion(LoginRequest request)
    {
        using HttpResponseMessage respuesta = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (!respuesta.IsSuccessStatusCode)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = "No se pudo iniciar sesión.";
            }

            throw new InvalidOperationException(mensaje);
        }

        LoginResponse? sesion = await respuesta.Content.ReadFromJsonAsync<LoginResponse>();

        if (sesion is null || string.IsNullOrWhiteSpace(sesion.Token) || string.IsNullOrWhiteSpace(sesion.RefreshToken))
        {
            throw new InvalidOperationException("El servidor no devolvió una sesión válida.");
        }

        await _almacenamientoTokensService.Guardar(sesion);
    }
}
