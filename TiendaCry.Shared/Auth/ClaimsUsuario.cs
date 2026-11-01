using System.Security.Claims;

namespace TiendaCry.Shared.Auth;

public static class ClaimsUsuario
{
    public const string EmailVerificado = "email_verificado";

    public static bool TieneEmailVerificado(ClaimsPrincipal usuario)
    {
        return bool.TryParse(usuario.FindFirst(EmailVerificado)?.Value, out bool verificado) && verificado;
    }
}
