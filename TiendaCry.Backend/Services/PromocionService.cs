using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Backend.Utilidades;
using TiendaCry.Shared.DTO.Promocion.Request;
using TiendaCry.Shared.DTO.Promocion.Response;

namespace TiendaCry.Backend.Services;

public class PromocionService
{
    private readonly IPromocionRepository _promocionRepository;
    private readonly ITiendaRepository _tiendaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<PromocionService> _logger;

    public PromocionService(IPromocionRepository promocionRepository, ITiendaRepository tiendaRepository, IUsuarioRepository usuarioRepository, ILogger<PromocionService> logger)
    {
        _promocionRepository = promocionRepository;
        _tiendaRepository = tiendaRepository;
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    public async Task<List<PromocionAdministradorResponse>> ObtenerAprobadas(int idTienda, int idUsuario, string baseUrl)
    {
        await ValidarAdministradorTiendaCry(idUsuario);

        List<Promocion> promociones = await _promocionRepository.ObtenerAprobadas(idTienda, DateTime.UtcNow);

        return CrearListadoAdministrador(promociones, baseUrl);
    }

    public async Task<List<PromocionAdministradorResponse>> ObtenerPendientes(int idUsuario, string baseUrl)
    {
        await ValidarAdministradorTiendaCry(idUsuario);

        List<Promocion> promociones = await _promocionRepository.ObtenerPendientes(DateTime.UtcNow);

        return CrearListadoAdministrador(promociones, baseUrl);
    }

    private List<PromocionAdministradorResponse> CrearListadoAdministrador(List<Promocion> promociones, string baseUrl)
    {
        return promociones.Select(p => new PromocionAdministradorResponse(
            p.Id,
            p.IdTienda,
            p.Tienda.Nombre,
            p.Titulo,
            p.Imagen is null ? null : $"{baseUrl.TrimEnd('/')}/{p.Imagen.RutaRelativa}")).ToList();
    }

    public async Task<bool> Revisar(int idTienda, int idPromocion, int idUsuario, RevisarPromocionRequest request)
    {
        await ValidarAdministradorTiendaCry(idUsuario);

        Promocion? promocion = await _promocionRepository.BuscarParaAdministracion(idPromocion);

        if (promocion is null || promocion.IdTienda != idTienda)
        {
            return false;
        }

        if (promocion.Tienda.EstadoTienda.Nombre != "Aprobada")
        {
            throw new DatosLlegaronErradosException("La tienda ya no está aprobada.");
        }

        if (promocion.EstadoPromocion.Nombre != "PendienteRevision")
        {
            throw new DatosLlegaronErradosException("La promoción ya no está pendiente de revisión.");
        }

        DateTime ahora = DateTime.UtcNow;

        if (promocion.FechaFin <= ahora)
        {
            throw new DatosLlegaronErradosException("La solicitud venció y ya no puede revisarse.");
        }

        string nombreEstado = "Aprobada";
        string? motivo = null;

        if (!request.Aprobar)
        {
            if (string.IsNullOrWhiteSpace(request.MotivoRechazo))
            {
                throw new DatosLlegaronErradosException("Ingresá el motivo del rechazo.");
            }

            nombreEstado = "Rechazada";
            motivo = request.MotivoRechazo.Trim();
        }

        int? idEstado = await _promocionRepository.ObtenerIdEstado(nombreEstado);

        if (idEstado is null)
        {
            throw new InvalidOperationException($"No está configurado el estado {nombreEstado} de promoción.");
        }

        promocion.IdEstadoPromocion = idEstado.Value;
        promocion.MotivoRechazo = motivo;
        promocion.FechaRevision = ahora;

        await _promocionRepository.GuardarCambios();

        _logger.LogInformation("El administrador {IdUsuario} revisó la promoción {IdPromocion}. Resultado: {Estado}.", idUsuario, idPromocion, nombreEstado);

        return true;
    }

    public async Task<bool> CancelarComoAdministrador(int idTienda, int idPromocion, int idUsuario)
    {
        await ValidarAdministradorTiendaCry(idUsuario);

        Promocion? promocion = await _promocionRepository.BuscarParaAdministracion(idPromocion);

        if (promocion is null || promocion.IdTienda != idTienda)
        {
            return false;
        }

        if (promocion.Tienda.EstadoTienda.Nombre != "Aprobada"
            || promocion.EstadoPromocion.Nombre != "Aprobada"
            || promocion.FechaFin <= DateTime.UtcNow)
        {
            throw new DatosLlegaronErradosException("La promoción ya no está aprobada o venció.");
        }

        int? idEstado = await _promocionRepository.ObtenerIdEstadoCancelada();

        if (idEstado is null)
        {
            throw new InvalidOperationException("No está configurado el estado Cancelada de promoción.");
        }

        promocion.IdEstadoPromocion = idEstado.Value;

        await _promocionRepository.GuardarCambios();

        _logger.LogInformation("El administrador de TiendaCry {IdUsuario} canceló la promoción {IdPromocion}.", idUsuario, idPromocion);

        return true;
    }

    private async Task ValidarAdministradorTiendaCry(int idUsuario)
    {
        Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

        if (usuario is null)
        {
            throw new CredencialesInvalidasException();
        }

        if (!usuario.EsAdministradorTiendaCry)
        {
            throw new SinPermisoException("No tenés permiso para gestionar promociones.");
        }

        if (!usuario.EmailVerificado)
        {
            throw new SinPermisoException("Verificá tu correo para continuar.");
        }

        if (usuario.DebeCambiarContrasena)
        {
            throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
        }
    }

    public async Task<DetallePromocionResponse?> ObtenerDetalle(int idTienda, int idPromocion, int idUsuario, string baseUrl)
    {
        Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

        if (usuario is null)
        {
            throw new CredencialesInvalidasException();
        }

        if (!usuario.EmailVerificado || usuario.DebeCambiarContrasena)
        {
            throw new SinPermisoException("Verificá tu correo y cambiá tu contraseña temporal si corresponde para continuar.");
        }

        if (!usuario.EsAdministradorTiendaCry)
        {
            await ValidarAdministrador(idTienda, idUsuario);
        }

        Promocion? promocion = await _promocionRepository.ObtenerDetalle(idTienda, idPromocion);

        if (promocion is null)
        {
            return null;
        }

        string? urlImagen = null;

        if (promocion.Imagen is not null)
        {
            urlImagen = $"{baseUrl.TrimEnd('/')}/{promocion.Imagen.RutaRelativa}";
        }

        return new DetallePromocionResponse(promocion.Titulo, promocion.Descripcion, urlImagen, promocion.FechaInicio, promocion.FechaFin);
    }

    public async Task<List<PromocionListadoResponse>> ObtenerPorTienda(int idTienda, int idUsuario, string baseUrl)
    {
        await ValidarAdministrador(idTienda, idUsuario);

        List<Promocion> promociones = await _promocionRepository.ObtenerPorTienda(idTienda);
        Dictionary<int, int> cantidades = await _promocionRepository.ObtenerCantidadUsos(idTienda);

        DateTime ahora = DateTime.UtcNow;

        return promociones.Select(p => new PromocionListadoResponse(
            p.Id,
            p.Titulo,
            p.EstadoPromocion.Nombre,
            p.Imagen is null ? null : $"{baseUrl.TrimEnd('/')}/{p.Imagen.RutaRelativa}",
            cantidades.GetValueOrDefault(p.Id),
            PuedeCancelar(p, ahora),
            p.FechaFin <= ahora)).ToList();
    }

    public async Task<RechazoPromocionResponse?> ObtenerRechazo(int idTienda, int idPromocion, int idUsuario)
    {
        await ValidarAdministrador(idTienda, idUsuario);

        Promocion? promocion = await _promocionRepository.BuscarPromocionAdministrable(idPromocion, idUsuario);

        if (promocion is null
            || promocion.IdTienda != idTienda
            || promocion.EstadoPromocion.Nombre != "Rechazada")
        {
            return null;
        }

        return new RechazoPromocionResponse(promocion.Titulo, promocion.MotivoRechazo);
    }

    public async Task<bool> Cancelar(int idTienda, int idPromocion, int idUsuario)
    {
        await ValidarAdministrador(idTienda, idUsuario);

        Promocion? promocion = await _promocionRepository.BuscarPromocionAdministrable(idPromocion, idUsuario);

        if (promocion is null || promocion.IdTienda != idTienda)
        {
            return false;
        }

        if (!PuedeCancelar(promocion, DateTime.UtcNow))
        {
            throw new DatosLlegaronErradosException("Solo podés cancelar solicitudes pendientes o promociones aprobadas que no hayan vencido.");
        }

        int? idEstadoCancelada = await _promocionRepository.ObtenerIdEstadoCancelada();

        if (idEstadoCancelada is null)
        {
            throw new InvalidOperationException("No está configurado el estado Cancelada de promoción.");
        }

        promocion.IdEstadoPromocion = idEstadoCancelada.Value;

        await _promocionRepository.GuardarCambios();

        _logger.LogInformation("El administrador {IdUsuario} canceló la promoción {IdPromocion} de la tienda {IdTienda}.", idUsuario, idPromocion, idTienda);

        return true;
    }

    private bool PuedeCancelar(Promocion promocion, DateTime ahora)
    {
        return promocion.EstadoPromocion.Nombre == "PendienteRevision"
            || (promocion.EstadoPromocion.Nombre == "Aprobada" && promocion.FechaFin > ahora);
    }

    private async Task ValidarAdministrador(int idTienda, int idUsuario)
    {
        if (!await _tiendaRepository.PuedeAdministrar(idTienda, idUsuario))
        {
            throw new SinPermisoException("No tenés permiso para gestionar promociones de esta tienda.");
        }
    }

    public async Task<PromocionCreadaResponse> SolicitarAlta(int idUsuario, SolicitarAltaPromocionRequest request)
    {
        if (!await _tiendaRepository.PuedeAdministrar(request.IdTienda, idUsuario))
        {
            throw new SinPermisoException("No tenés permiso para crear promociones en esta tienda.");
        }

        if (!Validaciones.EstanDatosBien(request.Titulo, request.Descripcion))
        {
            throw new DatosLlegaronErradosException("Ingresá el título y la descripción.");
        }

        if (request.FechaInicio == default || request.FechaFin == default)
        {
            throw new DatosLlegaronErradosException("Seleccioná las fechas de inicio y fin.");
        }

        if (request.FechaFin <= request.FechaInicio)
        {
            throw new DatosLlegaronErradosException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        int? idEstadoPendiente = await _promocionRepository.ObtenerIdEstadoPendiente();

        if (idEstadoPendiente is null)
        {
            throw new InvalidOperationException("No está configurado el estado PendienteRevision de promoción.");
        }

        Promocion promocion = new Promocion
        {
            Titulo = request.Titulo.Trim(),
            Descripcion = request.Descripcion.Trim(),
            FechaInicio = request.FechaInicio.UtcDateTime,
            FechaFin = request.FechaFin.UtcDateTime,
            FechaSolicitud = DateTime.UtcNow,
            IdTienda = request.IdTienda,
            IdEstadoPromocion = idEstadoPendiente.Value
        };

        await _promocionRepository.CrearSolicitud(promocion);

        _logger.LogInformation("El usuario {IdUsuario} solicitó el alta de la promoción {IdPromocion} en la tienda {IdTienda}.",
            idUsuario, promocion.Id, request.IdTienda);

        return new PromocionCreadaResponse(promocion.Id);
    }
}
