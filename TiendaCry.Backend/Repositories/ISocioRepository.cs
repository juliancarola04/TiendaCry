using TiendaCry.Backend.Models;

namespace TiendaCry.Backend.Repositories;

public interface ISocioRepository
{
    Task<Socio?> BuscarPorUsuario(int idUsuario);
    Task Crear(Socio socio);
    Task<bool> CiudadPerteneceAProvincia(int idCiudad, int idProvincia);
    Task GuardarCambios();
}
