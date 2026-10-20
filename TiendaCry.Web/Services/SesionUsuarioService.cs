using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Web.Services;

public class SesionUsuarioService
{
    // IMPORTANTE NO OLVIDAR PARA JULIÁN DEL FUTURO: Los datos del usuario van a vivir en un scoped de la pestaña en si.
    // Esto lo uso porque guardo el JWT en la cookie HttpOnly. Cosas que pasan por agregar seguridad.
    public UsuarioActualResponse? UsuarioActual { get; private set; }

    public void EstablecerUsuario(UsuarioActualResponse usuario)
    {
        UsuarioActual = usuario;
    }

    public void Limpiar()
    {
        UsuarioActual = null;
    }
}
