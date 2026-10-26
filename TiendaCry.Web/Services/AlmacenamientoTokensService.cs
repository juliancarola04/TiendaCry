using Microsoft.JSInterop;
using System.Text.Json;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Web.Services;

public class AlmacenamientoTokensService
{
    private readonly IJSRuntime _jsRuntime;

    public AlmacenamientoTokensService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task Guardar(AutenticacionResponse response)
    {
        string contenido = JsonSerializer.Serialize(response);

        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "sesion_usuario", contenido);
    }

    public async Task<AutenticacionResponse?> Obtener()
    {
        string? contenido = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "sesion_usuario");

        if (string.IsNullOrWhiteSpace(contenido))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<AutenticacionResponse>(contenido);
        }
        catch (JsonException)
        {
            await Limpiar();

            return null;
        }
    }

    public async Task Limpiar()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "sesion_usuario");
    }
}
