using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Repositories;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Utilidades;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Administrador.Request;
using TiendaCry.Shared.DTO.Usuario.Request;
using TiendaCry.Shared.DTO.Usuario.Response;

namespace TiendaCry.Backend.Services
{
    public class UsuarioService
    {
        private readonly ILogger<UsuarioService> _logger;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRegisterRepository _registerRepository;
        private readonly ITiendaRepository _tiendaRepository;
        private readonly ISesionUsuarioRepository _sesionRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository, IRegisterRepository registerRepository, ITiendaRepository tiendaRepository, ISesionUsuarioRepository sesionRepository, ILogger<UsuarioService> logger)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
            _registerRepository = registerRepository;
            _tiendaRepository = tiendaRepository;
            _sesionRepository = sesionRepository;
        }

        public async Task<List<UsuarioListadoResponse>> ObtenerUsuariosActivos(int idAdministrador)
        {
            await ValidarAdministrador(idAdministrador);

            List<Usuario> usuarios = await _usuarioRepository.ObtenerUsuariosActivos();

            return usuarios.Select(u => new UsuarioListadoResponse(u.Id, u.Nombre, u.Apellido, u.Email)).ToList();
        }

        public async Task CrearUsuario(int idAdministrador, CrearUsuarioRequest request)
        {
            await ValidarAdministrador(idAdministrador);

            if (!Validaciones.EstanDatosBien(request.Usuario, request.Email, request.ContrasenaTemporal, request.Nombre, request.Apellido))
            {
                throw new DatosLlegaronErradosException("Completá todos los campos.");
            }

            string username = request.Usuario.Trim();
            string email = request.Email.Trim();
            string nombre = request.Nombre.Trim();
            string apellido = request.Apellido.Trim();

            if (!Validaciones.CumpleLongitudMaxima(username, 30)
                || !Validaciones.CumpleLongitudMaxima(email, 254)
                || !Validaciones.CumpleLongitudMaxima(nombre, 35)
                || !Validaciones.CumpleLongitudMaxima(apellido, 35))
            {
                throw new DatosLlegaronErradosException("Alguno de los campos supera la longitud permitida.");
            }

            if (!Validaciones.EsUnEmailValido(email))
            {
                throw new DatosLlegaronErradosException("El correo ingresado no es válido.");
            }

            if (await _usuarioRepository.ExistePorUsername(username))
            {
                throw new RecursoExistenteException("Ya existe alguien con ese usuario.");
            }

            if (await _usuarioRepository.ExistePorEmail(email))
            {
                throw new RecursoExistenteException("Ya existe una cuenta con ese correo.");
            }

            int? idEstadoActivo = await _registerRepository.ObtenerIdEstadoActivo();

            if (idEstadoActivo is null)
            {
                throw new InvalidOperationException("No está configurado el estado Activo de usuario.");
            }

            Usuario usuario = new Usuario
            {
                NomUsuario = username,
                Email = email,
                Contrasena = BCrypt.Net.BCrypt.EnhancedHashPassword(request.ContrasenaTemporal),
                Nombre = nombre,
                Apellido = apellido,
                IdEstadoUsuario = idEstadoActivo.Value,
                EmailVerificado = false,
                DebeCambiarContrasena = true,
                EsAdministradorTiendaCry = false
            };

            await _registerRepository.Registrarse(usuario);

            _logger.LogInformation("El administrador {IdAdministrador} creó la cuenta del usuario {IdUsuario}.", idAdministrador, usuario.Id);
        }

        public async Task<List<UsuarioListadoResponse>> ObtenerAdministradoresActivos(int idAdministrador)
        {
            await ValidarAdministrador(idAdministrador);

            List<Usuario> usuarios = await _usuarioRepository.ObtenerAdministradoresActivos();

            return usuarios.Select(u => new UsuarioListadoResponse(u.Id, u.Nombre, u.Apellido, u.Email)).ToList();
        }

        public async Task CrearAdministrador(int idAdministrador, CrearAdministradorRequest request)
        {
            await ValidarAdministrador(idAdministrador);

            if (!Validaciones.EstanDatosBien(request.Usuario, request.Email, request.Contrasena, request.Nombre, request.Apellido))
            {
                throw new DatosLlegaronErradosException("Completá todos los campos.");
            }

            string username = request.Usuario.Trim();
            string email = request.Email.Trim();
            string nombre = request.Nombre.Trim();
            string apellido = request.Apellido.Trim();

            if (!Validaciones.CumpleLongitudMaxima(username, 30)
                || !Validaciones.CumpleLongitudMaxima(email, 254)
                || !Validaciones.CumpleLongitudMaxima(nombre, 35)
                || !Validaciones.CumpleLongitudMaxima(apellido, 35))
            {
                throw new DatosLlegaronErradosException("Alguno de los campos supera la longitud permitida.");
            }

            if (!Validaciones.EsUnEmailValido(email))
            {
                throw new DatosLlegaronErradosException("El correo ingresado no es válido.");
            }

            if (await _usuarioRepository.ExistePorUsername(username))
            {
                throw new RecursoExistenteException("Ya existe alguien con ese usuario.");
            }

            if (await _usuarioRepository.ExistePorEmail(email))
            {
                throw new RecursoExistenteException("Ya existe una cuenta con ese correo.");
            }

            int? idEstadoActivo = await _registerRepository.ObtenerIdEstadoActivo();

            if (idEstadoActivo is null)
            {
                throw new InvalidOperationException("No está configurado el estado Activo de usuario.");
            }

            Usuario usuario = new Usuario
            {
                NomUsuario = username,
                Email = email,
                Contrasena = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Contrasena),
                Nombre = nombre,
                Apellido = apellido,
                IdEstadoUsuario = idEstadoActivo.Value,
                EmailVerificado = false,
                DebeCambiarContrasena = false,
                EsAdministradorTiendaCry = true
            };

            await _registerRepository.Registrarse(usuario);

            _logger.LogInformation("El administrador {IdAdministrador} creó la cuenta del administrador {IdNuevoAdministrador}.", idAdministrador, usuario.Id);
        }

        public async Task<bool> DarDeBaja(int idUsuario, int idAdministrador)
        {
            await ValidarAdministrador(idAdministrador);

            if (idUsuario == idAdministrador)
            {
                throw new SinPermisoException("No podés dar de baja tu propia cuenta desde esta sección.");
            }

            Usuario? usuario = await _usuarioRepository.BuscarUsuarioActivoParaBaja(idUsuario);

            if (usuario is null)
            {
                return false;
            }

            int? idEstadoUsuarioBaja = await _usuarioRepository.ObtenerIdEstadoBaja();
            int? idEstadoTiendaBaja = await _tiendaRepository.ObtenerIdEstadoTienda("Baja");
            int? idEstadoVinculacionBaja = await _tiendaRepository.ObtenerIdEstadoUsuarioTienda("Baja");
            int? idEstadoPromocionCancelada = await _tiendaRepository.ObtenerIdEstadoPromocion("Cancelada");

            if (idEstadoUsuarioBaja is null || idEstadoTiendaBaja is null || idEstadoVinculacionBaja is null || idEstadoPromocionCancelada is null)
            {
                throw new InvalidOperationException("No están configurados los estados necesarios para dar de baja al usuario.");
            }

            List<Tienda> tiendas = usuario.UsuariosTienda
                .Where(ut => ut.RolTienda.Nombre == "Administrador"
                    && ut.EstadoUsuarioTienda.Nombre == "Activo"
                    && ut.FechaBaja == null)
                .Select(ut => ut.Tienda)
                .DistinctBy(t => t.Id)
                .ToList();

            List<SesionUsuario> sesiones = await _sesionRepository.ObtenerSesionesSinRevocar(idUsuario);
            DateTime ahora = DateTime.UtcNow;

            usuario.IdEstadoUsuario = idEstadoUsuarioBaja.Value;

            foreach (UsuarioTienda vinculacion in usuario.UsuariosTienda)
            {
                if (vinculacion.EstadoUsuarioTienda.Nombre == "Activo")
                {
                    vinculacion.IdEstadoUsuarioTienda = idEstadoVinculacionBaja.Value;
                    vinculacion.FechaBaja = ahora;
                }
            }

            foreach (Tienda tienda in tiendas)
            {
                tienda.IdEstadoTienda = idEstadoTiendaBaja.Value;

                foreach (UsuarioTienda vinculacion in tienda.UsuariosTienda)
                {
                    if (vinculacion.EstadoUsuarioTienda.Nombre == "Activo")
                    {
                        vinculacion.IdEstadoUsuarioTienda = idEstadoVinculacionBaja.Value;
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
            }

            foreach (SesionUsuario sesion in sesiones)
            {
                sesion.FechaRevocacion = ahora;
            }

            await _usuarioRepository.GuardarCambios();

            _logger.LogInformation("El administrador {IdAdministrador} dio de baja al usuario {IdUsuario}, sus vinculaciones y {CantidadTiendas} tiendas administradas. Se revocaron {CantidadSesiones} sesiones.", idAdministrador, idUsuario, tiendas.Count, sesiones.Count);

            return true;
        }

        public async Task<bool> DarDeBajaAdministrador(int idAdministrador, int idAdministradorActual)
        {
            await ValidarAdministrador(idAdministradorActual);

            if (idAdministrador == idAdministradorActual)
            {
                throw new SinPermisoException("No podés dar de baja tu propia cuenta desde esta sección.");
            }

            Usuario? administrador = await _usuarioRepository.BuscarAdministradorActivoParaBaja(idAdministrador);

            if (administrador is null)
            {
                return false;
            }

            int cantidadAdministradores = await _usuarioRepository.ContarAdministradoresActivos();

            if (cantidadAdministradores <= 1)
            {
                throw new DatosLlegaronErradosException("No se puede dar de baja al último administrador activo.");
            }

            int? idEstadoBaja = await _usuarioRepository.ObtenerIdEstadoBaja();

            if (idEstadoBaja is null)
            {
                throw new InvalidOperationException("No está configurado el estado Baja de usuario.");
            }

            List<SesionUsuario> sesiones = await _sesionRepository.ObtenerSesionesSinRevocar(idAdministrador);
            DateTime ahora = DateTime.UtcNow;

            administrador.IdEstadoUsuario = idEstadoBaja.Value;

            foreach (SesionUsuario sesion in sesiones)
            {
                sesion.FechaRevocacion = ahora;
            }

            await _usuarioRepository.GuardarCambios();

            _logger.LogInformation("El administrador {IdAdministradorActual} dio de baja al administrador {IdAdministrador}. Se revocaron {CantidadSesiones} sesiones.", idAdministradorActual, idAdministrador, sesiones.Count);

            return true;
        }

        private async Task ValidarAdministrador(int idAdministrador)
        {
            Usuario? administrador = await _usuarioRepository.BuscarPorId(idAdministrador);

            if (administrador is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (!administrador.EsAdministradorTiendaCry)
            {
                throw new SinPermisoException("No tenés permiso para gestionar usuarios.");
            }

            if (!administrador.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo para continuar.");
            }

            if (administrador.DebeCambiarContrasena)
            {
                throw new SinPermisoException("Cambiá tu contraseña temporal para continuar.");
            }
        }

        public async Task CambiarContrasenaTemporal(int idUsuario, CambiarContrasenaTemporalRequest request)
        {
            if (!Validaciones.EstanDatosBien(request.Contrasena))
            {
                throw new DatosLlegaronErradosException("Ingresá tu contraseña nueva.");
            }

            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null || !await _usuarioRepository.EstaActivo(idUsuario))
            {
                throw new CredencialesInvalidasException();
            }

            if (!usuario.EmailVerificado)
            {
                throw new SinPermisoException("Verificá tu correo antes de cambiar la contraseña temporal.");
            }

            if (!usuario.DebeCambiarContrasena)
            {
                throw new DatosLlegaronErradosException("La cuenta no requiere un cambio de contraseña temporal.");
            }

            string contrasena = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Contrasena);

            if (!await _usuarioRepository.CambiarContrasenaTemporal(idUsuario, contrasena))
            {
                throw new SinPermisoException("No se pudo cambiar la contraseña temporal de esta cuenta.");
            }

            _logger.LogInformation("El usuario {IdUsuario} cambió su contraseña temporal.", idUsuario);
        }

        public async Task ModificarEmailSinVerificar(int idUsuario, ModificarEmailRequest request)
        {
            if (!Validaciones.EstanDatosBien(request.Email))
            {
                throw new DatosLlegaronErradosException("Ingresá el nuevo correo.");
            }

            string email = request.Email.Trim();

            if (!Validaciones.CumpleLongitudMaxima(email, 254) || !Validaciones.EsUnEmailValido(email))
            {
                throw new DatosLlegaronErradosException("El correo ingresado no es válido.");
            }

            Usuario? usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (usuario.EmailVerificado)
            {
                throw new DatosLlegaronErradosException("Este cambio solo está disponible para correos sin verificar.");
            }

            if (usuario.Email == email)
            {
                throw new DatosLlegaronErradosException("Ingresá un correo diferente al actual.");
            }

            if (await _usuarioRepository.ExistePorEmail(email))
            {
                throw new RecursoExistenteException("Ya existe una cuenta con ese correo.");
            }

            bool modificado = await _usuarioRepository.ModificarEmailSinVerificar(idUsuario, email, DateTime.UtcNow);

            if (!modificado)
            {
                throw new DatosLlegaronErradosException("No se pudo modificar el correo de esta cuenta.");
            }

            _logger.LogInformation("El usuario {IdUsuario} modificó su correo sin verificar.", idUsuario);
        }

    }
}
