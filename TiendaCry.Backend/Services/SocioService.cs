using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.DTO.Socio.Request;
using TiendaCry.Shared.DTO.Socio.Response;

namespace TiendaCry.Backend.Services;

public class SocioService
{
    private readonly ISocioRepository _socioRepository;
    private readonly IPromocionRepository _promocionRepository;
    private readonly ILogger<SocioService> _logger;
    private readonly IUsuarioRepository _usuarioRepository;

    public SocioService(ISocioRepository socioRepository, IPromocionRepository promocionRepository, IUsuarioRepository usuarioRepository, ILogger<SocioService> logger)
    {
        _socioRepository = socioRepository;
        _promocionRepository = promocionRepository;
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }

    public async Task CrearSocio(int idUsuario)
    {
        Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

        if (usuario is null || !await _usuarioRepository.EstaActivo(idUsuario))
        {
            throw new CredencialesInvalidasException();
        }

        if (!usuario.EmailVerificado)
        {
            throw new SinPermisoException("Verificá tu correo para continuar.");
        }

        if (usuario.DebeCambiarContrasena)
        {
            throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
        }

        Socio? existente = await _socioRepository.BuscarPorUsuario(idUsuario);

        if (existente is not null)
        {
            return;
        }

        Socio socio = new Socio
        {
            IdUsuario = idUsuario,
            NumSocio = idUsuario.ToString("D8", System.Globalization.CultureInfo.InvariantCulture),
            IdCiudad = null
        };

        await _socioRepository.Crear(socio);

        _logger.LogInformation("El usuario {IdUsuario} se convirtió en socio. Socio {IdSocio}.", idUsuario, socio.Id);
    }

    public async Task<UbicacionSocioResponse> ObtenerUbicacion(int idUsuario)
    {
        Socio socio = await ObtenerSocioHabilitado(idUsuario);

        return new UbicacionSocioResponse(socio.Ciudad?.ProvinciaId, socio.IdCiudad);
    }

    public async Task GuardarUbicacion(int idUsuario, GuardarUbicacionRequest request)
    {
        Socio socio = await ObtenerSocioHabilitado(idUsuario);

        if (request.IdProvincia <= 0 || request.IdCiudad <= 0)
        {
            throw new DatosLlegaronErradosException("Seleccioná una provincia y una ciudad.");
        }

        if (!await _socioRepository.CiudadPerteneceAProvincia(request.IdCiudad, request.IdProvincia))
        {
            throw new DatosLlegaronErradosException("La ciudad seleccionada no pertenece a esa provincia.");
        }

        socio.IdCiudad = request.IdCiudad;

        await _socioRepository.GuardarCambios();

        _logger.LogInformation("El usuario {IdUsuario} actualizó la ubicación del socio {IdSocio}.", idUsuario, socio.Id);
    }

    public async Task<List<PromocionSocioResponse>> ObtenerPromociones(int idUsuario, bool cerca, string baseUrl)
    {
        Socio socio = await ObtenerSocioHabilitado(idUsuario);
        int? idProvincia = null;
        int? idCiudad = null;

        if (cerca)
        {
            if (!socio.IdCiudad.HasValue || socio.Ciudad is null)
            {
                throw new DatosLlegaronErradosException("Seleccioná tu ubicación para filtrar las promociones.");
            }

            idProvincia = socio.Ciudad.ProvinciaId;
            idCiudad = socio.IdCiudad.Value;
        }

        List<Promocion> promociones = await _promocionRepository.ObtenerActivasParaSocio(idProvincia, idCiudad, DateTime.UtcNow);

        return promociones.Select(p => new PromocionSocioResponse(
            p.Id,
            p.Titulo,
            p.Imagen is null ? null : $"{baseUrl.TrimEnd('/')}/{p.Imagen.RutaRelativa}")).ToList();
    }

    public async Task<DetallePromocionSocioResponse?> ObtenerDetalle(int idUsuario, int idPromocion, string baseUrl)
    {
        await ObtenerSocioHabilitado(idUsuario);

        Promocion? promocion = await _promocionRepository.ObtenerDetalleActivoParaSocio(idPromocion, DateTime.UtcNow);

        if (promocion is null)
        {
            return null;
        }

        string? urlImagen = null;

        if (promocion.Imagen is not null)
        {
            urlImagen = $"{baseUrl.TrimEnd('/')}/{promocion.Imagen.RutaRelativa}";
        }

        return new DetallePromocionSocioResponse(promocion.Titulo, promocion.Descripcion, urlImagen, promocion.Tienda.Ciudad.Provincia.Nombre, promocion.Tienda.Ciudad.Nombre, promocion.Tienda.Calle, promocion.Tienda.NumCalle, promocion.FechaInicio, promocion.FechaFin);
    }

    private async Task<Socio> ObtenerSocioHabilitado(int idUsuario)
    {
        Socio? socio = await _socioRepository.BuscarPorUsuario(idUsuario);

        if (socio is null)
        {
            throw new SinPermisoException("No estás registrado como socio.");
        }

        if (socio.Usuario.EstadoUsuario.Nombre != "Activo")
        {
            throw new CredencialesInvalidasException();
        }

        if (!socio.Usuario.EmailVerificado)
        {
            throw new SinPermisoException("Verificá tu correo para continuar.");
        }

        if (socio.Usuario.DebeCambiarContrasena)
        {
            throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
        }

        return socio;
    }
}
