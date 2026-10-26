using Microsoft.Maui.Storage;
using System.Text.Json;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Mobile.Services;

public class AlmacenamientoTokensService
{
    public async Task Guardar(AutenticacionResponse response)
    {
        string contenido = JsonSerializer.Serialize(response);

        await SecureStorage.Default.SetAsync("sesion_usuario", contenido);
    }

    public async Task<AutenticacionResponse?> Obtener()
    {
        string? contenido = await SecureStorage.Default.GetAsync("sesion_usuario");

        if (string.IsNullOrWhiteSpace(contenido))
        {
            return null;
        }

        return JsonSerializer.Deserialize<AutenticacionResponse>(contenido);
    }

    public void Limpiar()
    {
        SecureStorage.Default.Remove("sesion_usuario");
    }
}
