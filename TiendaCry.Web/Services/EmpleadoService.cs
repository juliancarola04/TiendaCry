using System.Net;
using System.Net.Http.Json;
using TiendaCry.Shared.DTO.Empleado.Request;
using TiendaCry.Shared.DTO.Empleado.Response;

namespace TiendaCry.Web.Services;

public class EmpleadoService
{
    private readonly AutenticacionService _autenticacionService;

    public EmpleadoService(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
    }

    public async Task<List<EmpleadoResponse>> ObtenerEmpleados(int idTienda)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Get, $"api/tiendas/{idTienda}/empleados"));
        await ValidarRespuesta(respuesta, "No se pudieron obtener los empleados.");

        return await respuesta.Content.ReadFromJsonAsync<List<EmpleadoResponse>>() ?? throw new InvalidOperationException("No se pudieron obtener los empleados.");
    }

    public async Task<ConsultarCorreoEmpleadoResponse> ConsultarCorreo(int idTienda, ConsultarCorreoEmpleadoRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, $"api/tiendas/{idTienda}/empleados/consultar-correo")
        {
            Content = JsonContent.Create(request)
        });
        await ValidarRespuesta(respuesta, "No se pudo consultar el correo.");

        return await respuesta.Content.ReadFromJsonAsync<ConsultarCorreoEmpleadoResponse>() ?? throw new InvalidOperationException("No se pudo consultar el correo.");
    }

    public async Task AgregarEmpleado(int idTienda, AgregarEmpleadoRequest request)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Post, $"api/tiendas/{idTienda}/empleados")
        {
            Content = JsonContent.Create(request)
        });

        await ValidarRespuesta(respuesta, "No se pudo agregar al empleado.");
    }

    public async Task DarDeBaja(int idTienda, int idUsuarioTienda)
    {
        using HttpResponseMessage respuesta = await _autenticacionService.EnviarAutenticado(() => new HttpRequestMessage(HttpMethod.Put, $"api/tiendas/{idTienda}/empleados/{idUsuarioTienda}/baja"));

        await ValidarRespuesta(respuesta, "No se pudo dar de baja al empleado.");
    }

    private async Task ValidarRespuesta(HttpResponseMessage respuesta, string mensajeError)
    {
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized || respuesta.StatusCode == HttpStatusCode.Forbidden || respuesta.StatusCode == HttpStatusCode.NotFound)
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
