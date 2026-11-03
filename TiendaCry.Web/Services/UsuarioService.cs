using System.Net;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Usuario.Request;
using TiendaCry.Shared.DTO.Usuario.Response;

namespace TiendaCry.Web.Services;

public class UsuarioService
{
    private readonly AutenticacionService _autenticacionService;

    public UsuarioService(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task<List<UsuarioListadoResponse>> ObtenerUsuarios()
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, "api/administracion/usuarios"));

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<List<UsuarioListadoResponse>>()
            ?? throw new InvalidOperationException("No se pudieron obtener los usuarios.");
    }

    public async Task DarDeBaja(int idUsuario)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, $"api/administracion/usuarios/{idUsuario}/baja"));

        respuesta.EnsureSuccessStatusCode();
    }

    public async Task CrearUsuario(CrearUsuarioRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, "api/administracion/usuarios")
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
                mensaje = "No se pudo crear el usuario.";
            }

            throw new InvalidOperationException(mensaje);
        }
    }
}
