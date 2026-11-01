using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Backend.Utilidades;
using TiendaCry.Shared.DTO.Empleado.Request;
using TiendaCry.Shared.DTO.Empleado.Response;

namespace TiendaCry.Backend.Services;

public class EmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRegisterRepository _registerRepository;

    public EmpleadoService(IEmpleadoRepository empleadoRepository, IUsuarioRepository usuarioRepository, IRegisterRepository registerRepository)
    {
        _empleadoRepository = empleadoRepository;
        _usuarioRepository = usuarioRepository;
        _registerRepository = registerRepository;
    }

    public async Task<List<EmpleadoResponse>> ObtenerEmpleados(int idTienda, int idUsuario)
    {
        await ValidarAdministrador(idTienda, idUsuario);

        List<UsuarioTienda> empleados = await _empleadoRepository.ObtenerEmpleados(idTienda);

        return empleados.Select(ut => new EmpleadoResponse(ut.Id, ut.Usuario.Nombre, ut.Usuario.Apellido)).ToList();
    }

    public async Task<ConsultarCorreoEmpleadoResponse> ConsultarCorreo(int idTienda, int idUsuario, ConsultarCorreoEmpleadoRequest request)
    {
        await ValidarAdministrador(idTienda, idUsuario);

        string email = ValidarCorreo(request.Email);
        Usuario? usuario = await _usuarioRepository.BuscarPorEmail(email);

        if (usuario is null)
        {
            return new ConsultarCorreoEmpleadoResponse(false);
        }

        await ValidarUsuarioExistente(idTienda, usuario);

        return new ConsultarCorreoEmpleadoResponse(true);
    }

    public async Task AgregarEmpleado(int idTienda, int idUsuario, AgregarEmpleadoRequest request)
    {
        await ValidarAdministrador(idTienda, idUsuario);

        string email = ValidarCorreo(request.Email);
        Usuario? usuario = await _usuarioRepository.BuscarPorEmail(email);

        if (usuario is not null)
        {
            await ValidarUsuarioExistente(idTienda, usuario);
        }
        else
        {
            if (!Validaciones.EstanDatosBien(request.Usuario, request.ContrasenaTemporal, request.Nombre, request.Apellido))
            {
                throw new DatosLlegaronErradosException("Completá los datos para crear la cuenta del empleado.");
            }

            string username = request.Usuario!.Trim();
            string nombre = request.Nombre!.Trim();
            string apellido = request.Apellido!.Trim();

            if (!Validaciones.CumpleLongitudMaxima(username, 30)
                || !Validaciones.CumpleLongitudMaxima(nombre, 35)
                || !Validaciones.CumpleLongitudMaxima(apellido, 35))
            {
                throw new DatosLlegaronErradosException("Alguno de los campos supera la longitud permitida.");
            }

            if (await _usuarioRepository.ExistePorUsername(username))
            {
                throw new RecursoExistenteException("Ya existe alguien con ese usuario.");
            }

            int? idEstadoUsuarioActivo = await _registerRepository.ObtenerIdEstadoActivo();

            if (idEstadoUsuarioActivo is null)
            {
                throw new InvalidOperationException("No está configurado el estado Activo de usuario.");
            }

            usuario = new Usuario
            {
                NomUsuario = username,
                Email = email,
                Nombre = nombre,
                Apellido = apellido,
                Contrasena = BCrypt.Net.BCrypt.EnhancedHashPassword(request.ContrasenaTemporal!),
                IdEstadoUsuario = idEstadoUsuarioActivo.Value,
                EmailVerificado = false,
                DebeCambiarContrasena = true,
                EsAdministradorTiendaCry = false
            };
        }

        int? idRolEmpleado = await _empleadoRepository.ObtenerIdRolEmpleado();
        int? idEstadoActivo = await _empleadoRepository.ObtenerIdEstadoActivo();

        if (idRolEmpleado is null || idEstadoActivo is null)
        {
            throw new InvalidOperationException("No están configurados el rol o el estado del empleado.");
        }

        UsuarioTienda vinculacion = new UsuarioTienda
        {
            IdTienda = idTienda,
            IdRolTienda = idRolEmpleado.Value,
            IdEstadoUsuarioTienda = idEstadoActivo.Value,
            FechaAlta = DateTime.UtcNow
        };

        if (usuario.Id == 0)
        {
            vinculacion.Usuario = usuario;
        }
        else
        {
            vinculacion.IdUsuario = usuario.Id;
        }

        await _empleadoRepository.Agregar(vinculacion);
    }

    public async Task<bool> DarDeBaja(int idTienda, int idUsuarioTienda, int idUsuario)
    {
        await ValidarAdministrador(idTienda, idUsuario);

        UsuarioTienda? empleado = await _empleadoRepository.BuscarEmpleadoActivo(idTienda, idUsuarioTienda);

        if (empleado is null)
        {
            return false;
        }

        int? idEstadoBaja = await _empleadoRepository.ObtenerIdEstadoBaja();

        if (idEstadoBaja is null)
        {
            throw new InvalidOperationException("No está configurado el estado Baja de empleado.");
        }

        empleado.IdEstadoUsuarioTienda = idEstadoBaja.Value;
        empleado.FechaBaja = DateTime.UtcNow;

        await _empleadoRepository.GuardarCambios();

        return true;
    }

    private async Task ValidarAdministrador(int idTienda, int idUsuario)
    {
        if (!await _empleadoRepository.EsAdministradorDeTienda(idTienda, idUsuario))
        {
            throw new SinPermisoException("No tenés permiso para gestionar los empleados de esta tienda.");
        }
    }

    private async Task ValidarUsuarioExistente(int idTienda, Usuario usuario)
    {
        if (usuario.EstadoUsuario.Nombre != "Activo")
        {
            throw new DatosLlegaronErradosException("La cuenta asociada a ese correo está dada de baja.");
        }

        if (await _empleadoRepository.TieneVinculacionActiva(idTienda, usuario.Id))
        {
            throw new RecursoExistenteException("El usuario ya está vinculado a esta tienda.");
        }
    }

    private string ValidarCorreo(string correo)
    {
        if (!Validaciones.EstanDatosBien(correo))
        {
            throw new DatosLlegaronErradosException("Ingresá un correo.");
        }

        string email = correo.Trim();

        if (!Validaciones.CumpleLongitudMaxima(email, 254) || !Validaciones.EsUnEmailValido(email))
        {
            throw new DatosLlegaronErradosException("El correo ingresado no es válido.");
        }

        return email;
    }
}
