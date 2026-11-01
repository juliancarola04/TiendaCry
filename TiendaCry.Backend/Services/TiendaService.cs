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
        private readonly ITiendaRepository _tiendaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public TiendaService(ITiendaRepository tiendaRepository, IUsuarioRepository usuarioRepository)
        {
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

            return tiendas.Select(t => new SolicitudTiendaPendienteResponse(t.Id, t.Nombre, t.FechaSolicitud)).ToList();
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

            return new SolicitudTiendaResponse(tienda.Id, tienda.Nombre, "PendienteRevision", null);
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
        }
    }
}
