using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net;
using System.Security.Claims;
using TiendaCry.Web.Services;

namespace TiendaCry.Web.Auth;

public class EstadoAutenticacionProvider : AuthenticationStateProvider
{
    private readonly AutenticacionService _autenticacionService;

    public EstadoAutenticacionProvider(AutenticacionService autenticacionService)
    {
        _autenticacionService = autenticacionService;
        _autenticacionService.SesionCambio += Actualizar;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal usuario = new ClaimsPrincipal(new ClaimsIdentity());

        try
        {
            string? token = await _autenticacionService.ObtenerTokenVigente();

            if (!string.IsNullOrWhiteSpace(token))
            {
                JsonWebTokenHandler handler = new JsonWebTokenHandler();
                JsonWebToken jwt = handler.ReadJsonWebToken(token);

                ClaimsIdentity identidad = new ClaimsIdentity(jwt.Claims, "Bearer", "sub", ClaimTypes.Role);

                usuario = new ClaimsPrincipal(identidad);
            }
        }
        catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.Unauthorized)
        {
            usuario = new ClaimsPrincipal(new ClaimsIdentity());
        }

        return new AuthenticationState(usuario);
    }

    private void Actualizar()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
