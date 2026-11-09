using System.Net;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Administrador.Request;
using TiendaCry.Shared.DTO.Usuario.Response;

namespace TiendaCry.Web.Services;

public class AdministradorService
{
    private readonly AutenticacionService _autenticacionService;

    public AdministradorService(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task<List<UsuarioListadoResponse>> ObtenerAdministradores()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/administracion/administradores"));

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<List<UsuarioListadoResponse>>()
            ?? throw new InvalidOperationException("No se pudieron obtener los administradores.");
    }

    public async Task CrearAdministrador(CrearAdministradorRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, "api/administracion/administradores")
        {
            Content = JsonContent.Create(request)
        });

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized || respuesta.StatusCode == HttpStatusCode.Forbidden)
        {
            respuesta.EnsureSuccessStatusCode();
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = "No se pudo crear el administrador.";
            }

            throw new InvalidOperationException(mensaje);
        }
    }

    public async Task DarDeBaja(int idAdministrador)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, $"api/administracion/administradores/{idAdministrador}/baja"));

        if (respuesta.StatusCode == HttpStatusCode.BadRequest
            || respuesta.StatusCode == HttpStatusCode.Forbidden
            || respuesta.StatusCode == HttpStatusCode.NotFound)
        {
            string mensaje = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(mensaje))
            {
                mensaje = "No se pudo dar de baja al administrador.";
            }

            throw new InvalidOperationException(mensaje);
        }

        respuesta.EnsureSuccessStatusCode();
    }
}
