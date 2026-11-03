using TiendaCry.Backend.Utilidades;
using TiendaCry.Shared.DTO.Tienda.Request;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.DTO.Tienda.Response;

namespace TiendaCry.Backend.Services
{
    public class TiendaService
    {
        private readonly ILogger<TiendaService> _logger;
        private readonly ITiendaRepository _tiendaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public TiendaService(ITiendaRepository tiendaRepository, IUsuarioRepository usuarioRepository, ILogger<TiendaService> logger)
        {
            _logger = logger;
            _tiendaRepository = tiendaRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<TiendaActivaResponse?> ObtenerTiendaActiva(int idUsuario)
        {
            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (!usuario.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo para consultar tu tienda activa.");
            }

            if (usuario.DebeCambiarContrasena)
            {
                throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
            }

            Tienda? tienda = await _tiendaRepository.BuscarTiendaActivaPorAdministrador(idUsuario);

            if (tienda is null)
            {
                return null;
            }

            return new TiendaActivaResponse(tienda.Id, tienda.Nombre);
        }

        public async Task<List<TiendaActivaResponse>> ObtenerTiendasAprobadas(int idUsuario)
        {
            await ValidarAdministrador(idUsuario);

            List<Tienda> tiendas = await _tiendaRepository.ObtenerTiendasAprobadas();

            return tiendas.Select(t => new TiendaActivaResponse(t.Id, t.Nombre)).ToList();
        }

        public async Task<DetalleTiendaResponse?> ObtenerDetalleTiendaAprobada(int idTienda, int idUsuario)
        {
            await ValidarAdministrador(idUsuario);

            Tienda? tienda = await _tiendaRepository.BuscarTiendaAprobada(idTienda);

            if (tienda is null)
            {
                return null;
            }

            return new DetalleTiendaResponse(
                tienda.Id,
                tienda.Nombre,
                tienda.Ciudad.Provincia.Nombre,
                tienda.Ciudad.Nombre,
                tienda.Categoria.Nombre,
                tienda.Calle,
                tienda.NumCalle);
        }

        public async Task<List<SolicitudTiendaPendienteResponse>> ObtenerSolicitudesPendientes(int idUsuario)
        {
            await ValidarAdministrador(idUsuario);

            List<Tienda> tiendas = await _tiendaRepository.ObtenerSolicitudesPendientes();

            return tiendas.Select(t => new SolicitudTiendaPendienteResponse(t.Id, t.Nombre)).ToList();
        }

        public async Task<DetalleTiendaResponse?> ObtenerDetalleSolicitudPendiente(int idTienda, int idUsuario)
        {
            await ValidarAdministrador(idUsuario);

            Tienda? tienda = await _tiendaRepository.BuscarSolicitudPendiente(idTienda);

            if (tienda is null)
            {
                return null;
            }

            return new DetalleTiendaResponse(
                tienda.Id,
                tienda.Nombre,
                tienda.Ciudad.Provincia.Nombre,
                tienda.Ciudad.Nombre,
                tienda.Categoria.Nombre,
                tienda.Calle,
                tienda.NumCalle);
        }

        public async Task<List<SolicitudTiendaResponse>> ObtenerSolicitudes(int idUsuario)
        {
            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (!usuario.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo para consultar las solicitudes de tiendas.");
            }

            if (usuario.DebeCambiarContrasena)
            {
                throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
            }

            List<Tienda> tiendas = await _tiendaRepository.ObtenerSolicitudesPorUsuario(idUsuario);

            return tiendas.Select(t => new SolicitudTiendaResponse(
                t.Id,
                t.Nombre,
                t.EstadoTienda.Nombre,
                t.EstadoTienda.Nombre == "Rechazada" ? t.MotivoRechazo : null))
                .ToList();
        }
        public async Task<SolicitudTiendaResponse> SolicitarAlta(int idUsuario, SolicitarAltaTiendaRequest request)
        {
            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (!usuario.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo para solicitar el alta de una tienda.");
            }

            if (usuario.DebeCambiarContrasena)
            {
                throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
            }

            if (!Validaciones.EstanDatosBien(request.Nombre, request.Calle, request.NumCalle))
            {
                throw new DatosLlegaronErradosException("Todos los campos son obligatorios.");
            }

            if (request.IdCiudad <= 0 || request.IdCategoria <= 0)
            {
                throw new DatosLlegaronErradosException("Seleccioná una ciudad y una categoría.");
            }

            if (await _tiendaRepository.TieneTiendaComoAdministrador(idUsuario))
            {
                throw new RecursoExistenteException("Ya tenés una tienda asociada como administrador. Consultá el estado de tu solicitud.");
            }

            if (!await _tiendaRepository.ExisteCiudad(request.IdCiudad))
            {
                throw new DatosLlegaronErradosException("La ciudad seleccionada no existe.");
            }

            if (!await _tiendaRepository.ExisteCategoria(request.IdCategoria))
            {
                throw new DatosLlegaronErradosException("La categoría seleccionada no existe.");
            }

            int? idEstadoPendiente = await _tiendaRepository.ObtenerIdEstadoPendiente();
            int? idRolAdministrador = await _tiendaRepository.ObtenerIdRolAdministrador();
            int? idEstadoActivo = await _tiendaRepository.ObtenerIdEstadoVinculacionActivo();

            if (idEstadoPendiente is null || idRolAdministrador is null || idEstadoActivo is null)
            {
                throw new InvalidOperationException("No están configurados los estados o el rol necesarios para solicitar el alta de una tienda.");
            }

            DateTime ahora = DateTime.UtcNow;

            Tienda tienda = new Tienda
            {
                Nombre = request.Nombre.Trim(),
                Calle = request.Calle.Trim(),
                NumCalle = request.NumCalle.Trim(),
                IdCiudad = request.IdCiudad,
                IdCategoria = request.IdCategoria,
                IdEstadoTienda = idEstadoPendiente.Value,
                FechaSolicitud = ahora
            };

            tienda.UsuariosTienda.Add(new UsuarioTienda
            {
                IdUsuario = idUsuario,
                IdRolTienda = idRolAdministrador.Value,
                IdEstadoUsuarioTienda = idEstadoActivo.Value,
                FechaAlta = ahora
            });

            await _tiendaRepository.CrearSolicitud(tienda);

            _logger.LogInformation("El usuario {IdUsuario} solicitó el alta de la tienda {IdTienda}.", idUsuario, tienda.Id);

            return new SolicitudTiendaResponse(tienda.Id, tienda.Nombre, "PendienteRevision", null);
        }

        public async Task<DetalleSolicitudTiendaResponse?> ObtenerSolicitudRechazada(int idTienda, int idUsuario)
        {
            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (!usuario.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo para consultar la solicitud de la tienda.");
            }

            if (usuario.DebeCambiarContrasena)
            {
                throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
            }

            Tienda? tienda = await _tiendaRepository.BuscarSolicitudRechazada(idTienda, idUsuario);

            if (tienda is null)
            {
                return null;
            }

            return new DetalleSolicitudTiendaResponse(
                tienda.Id,
                tienda.Nombre,
                tienda.Calle,
                tienda.NumCalle,
                tienda.Ciudad.ProvinciaId,
                tienda.IdCiudad,
                tienda.IdCategoria,
                tienda.MotivoRechazo);
        }

        public async Task<SolicitudTiendaResponse?> ReenviarSolicitud(int idTienda, int idUsuario, ReenviarSolicitudTiendaRequest request)
        {
            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (!usuario.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo para reenviar la solicitud de la tienda.");
            }

            if (usuario.DebeCambiarContrasena)
            {
                throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
            }

            Tienda? tienda = await _tiendaRepository.BuscarSolicitudRechazada(idTienda, idUsuario);

            if (tienda is null)
            {
                return null;
            }

            if (!Validaciones.EstanDatosBien(request.Nombre, request.Calle, request.NumCalle))
            {
                throw new DatosLlegaronErradosException("Todos los campos son obligatorios.");
            }

            if (request.IdCiudad <= 0 || request.IdCategoria <= 0)
            {
                throw new DatosLlegaronErradosException("Seleccioná una ciudad y una categoría.");
            }

            if (!await _tiendaRepository.ExisteCiudad(request.IdCiudad))
            {
                throw new DatosLlegaronErradosException("La ciudad seleccionada no existe.");
            }

            if (!await _tiendaRepository.ExisteCategoria(request.IdCategoria))
            {
                throw new DatosLlegaronErradosException("La categoría seleccionada no existe.");
            }

            int? idEstadoPendiente = await _tiendaRepository.ObtenerIdEstadoPendiente();

            if (idEstadoPendiente is null)
            {
                throw new InvalidOperationException("No está configurado el estado PendienteRevision de tienda.");
            }

            tienda.Nombre = request.Nombre.Trim();
            tienda.Calle = request.Calle.Trim();
            tienda.NumCalle = request.NumCalle.Trim();
            tienda.IdCiudad = request.IdCiudad;
            tienda.IdCategoria = request.IdCategoria;
            tienda.IdEstadoTienda = idEstadoPendiente.Value;
            tienda.FechaSolicitud = DateTime.UtcNow;
            tienda.FechaRevision = null;
            tienda.MotivoRechazo = null;

            await _tiendaRepository.GuardarCambios();

            _logger.LogInformation("El usuario {IdUsuario} reenvió la solicitud de la tienda {IdTienda}.", idUsuario, idTienda);

            return new SolicitudTiendaResponse(tienda.Id, tienda.Nombre, "PendienteRevision", null);
        }

        public async Task<bool> AceptarSolicitud(int idTienda, int idUsuario)
        {
            return await ResolverSolicitud(idTienda, idUsuario, "Aprobada", null);
        }

        public async Task<bool> RechazarSolicitud(int idTienda, int idUsuario, RechazarSolicitudTiendaRequest request)
        {
            if (!Validaciones.EstanDatosBien(request.MotivoRechazo))
            {
                throw new DatosLlegaronErradosException("Ingresá el motivo del rechazo.");
            }

            return await ResolverSolicitud(idTienda, idUsuario, "Rechazada", request.MotivoRechazo.Trim());
        }

        private async Task<bool> ResolverSolicitud(int idTienda, int idUsuario, string estado, string? motivoRechazo)
        {
            await ValidarAdministrador(idUsuario);

            Tienda? tienda = await _tiendaRepository.BuscarSolicitudPendienteParaRevision(idTienda);

            if (tienda is null)
            {
                return false;
            }

            int? idEstado = await _tiendaRepository.ObtenerIdEstadoTienda(estado);

            if (idEstado is null)
            {
                throw new InvalidOperationException($"No está configurado el estado {estado} de tienda.");
            }

            tienda.IdEstadoTienda = idEstado.Value;
            tienda.FechaRevision = DateTime.UtcNow;
            tienda.MotivoRechazo = motivoRechazo;

            await _tiendaRepository.GuardarCambios();

            _logger.LogInformation("El administrador {IdAdministrador} cambió la solicitud de la tienda {IdTienda} al estado {Estado}.", idUsuario, idTienda, estado);

            return true;
        }

        public async Task<bool> DarDeBaja(int idTienda, int idUsuario)
        {
            await ValidarAdministrador(idUsuario);

            return await AplicarBaja(idTienda, idUsuario);
        }

        public async Task<bool> CancelarRelaciones(int idTienda, int idUsuario)
        {
            if (!await _tiendaRepository.PuedeAdministrar(idTienda, idUsuario))
            {
                throw new SinPermisoException("No tenés permiso para cancelar las relaciones de esta tienda.");
            }

            return await AplicarBaja(idTienda, idUsuario);
        }

        private async Task<bool> AplicarBaja(int idTienda, int idUsuario)
        {
            Tienda? tienda = await _tiendaRepository.BuscarTiendaAprobadaParaBaja(idTienda);

            if (tienda is null)
            {
                return false;
            }

            int? idEstadoTiendaBaja = await _tiendaRepository.ObtenerIdEstadoTienda("Baja");
            int? idEstadoUsuarioTiendaBaja = await _tiendaRepository.ObtenerIdEstadoUsuarioTienda("Baja");
            int? idEstadoPromocionCancelada = await _tiendaRepository.ObtenerIdEstadoPromocion("Cancelada");

            if (idEstadoTiendaBaja is null || idEstadoUsuarioTiendaBaja is null || idEstadoPromocionCancelada is null)
            {
                throw new InvalidOperationException("No están configurados los estados necesarios para dar de baja la tienda.");
            }

            DateTime ahora = DateTime.UtcNow;

            tienda.IdEstadoTienda = idEstadoTiendaBaja.Value;

            foreach (UsuarioTienda vinculacion in tienda.UsuariosTienda)
            {
                if (vinculacion.EstadoUsuarioTienda.Nombre == "Activo")
                {
                    vinculacion.IdEstadoUsuarioTienda = idEstadoUsuarioTiendaBaja.Value;
                    vinculacion.FechaBaja = ahora;
                }
            }

            foreach (Promocion promocion in tienda.Promociones)
            {
                if (promocion.EstadoPromocion.Nombre == "Aprobada" || promocion.EstadoPromocion.Nombre == "PendienteRevision")
                {
                    promocion.IdEstadoPromocion = idEstadoPromocionCancelada.Value;
                }
            }

            await _tiendaRepository.GuardarCambios();

            _logger.LogInformation("El administrador {IdAdministrador} dio de baja la tienda {IdTienda} y actualizó sus vinculaciones y promociones.", idUsuario, idTienda);

            return true;
        }

        private async Task ValidarAdministrador(int idUsuario)
        {
            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (!usuario.EsAdministradorTiendaCry)
            {
                throw new SinPermisoException("No tenés permiso para gestionar tiendas.");
            }

            if (!usuario.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo para gestionar tiendas.");
            }

            if (usuario.DebeCambiarContrasena)
            {
                throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
            }
        }
    }
}
