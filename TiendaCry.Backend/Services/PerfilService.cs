using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Options;
using TiendaCry.Backend.Repositories;
using TiendaCry.Backend.Utilidades;
using TiendaCry.Shared.DTO.Auth.Request;
using TiendaCry.Shared.DTO.Perfil.Request;
using TiendaCry.Shared.DTO.Perfil.Response;

namespace TiendaCry.Backend.Services;

public class PerfilService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenEmailRepository _tokenEmailRepository;
    private readonly EmailService _emailService;
    private readonly EmailSettings _settings;
    private readonly ILogger<PerfilService> _logger;

    public PerfilService(IUsuarioRepository usuarioRepository, ITokenEmailRepository tokenEmailRepository, EmailService emailService, IOptions<EmailSettings> settings, ILogger<PerfilService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _tokenEmailRepository = tokenEmailRepository;
        _emailService = emailService;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<PerfilResponse> ObtenerPerfil(int idUsuario)
    {
        Usuario usuario = await ObtenerUsuarioHabilitado(idUsuario);

        return new PerfilResponse(usuario.Email);
    }

    public async Task CambiarContrasena(int idUsuario, CambiarContrasenaRequest request)
    {
        await ObtenerUsuarioHabilitado(idUsuario);

        if (!Validaciones.EstanDatosBien(request.Contrasena, request.ConfirmacionContrasena))
        {
            throw new DatosLlegaronErradosException("Completá ambos campos.");
        }

        if (request.Contrasena != request.ConfirmacionContrasena)
        {
            throw new DatosLlegaronErradosException("Las contraseñas no coinciden.");
        }

        string contrasena = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Contrasena);

        if (!await _usuarioRepository.CambiarContrasena(idUsuario, contrasena, DateTime.UtcNow))
        {
            throw new SinPermisoException("No se pudo cambiar la contraseña de esta cuenta.");
        }

        _logger.LogInformation("El usuario {IdUsuario} cambió su contraseña desde el perfil.", idUsuario);
    }

    public async Task SolicitarCambioEmail(int idUsuario, ModificarEmailRequest request)
    {
        Usuario usuario = await ObtenerUsuarioHabilitado(idUsuario);

        if (!Validaciones.EstanDatosBien(request.Email))
        {
            throw new DatosLlegaronErradosException("Ingresá el nuevo correo.");
        }

        string emailNuevo = request.Email.Trim();

        if (!Validaciones.CumpleLongitudMaxima(emailNuevo, 254) || !Validaciones.EsUnEmailValido(emailNuevo))
        {
            throw new DatosLlegaronErradosException("El correo ingresado no es válido.");
        }

        if (string.Equals(usuario.Email, emailNuevo, StringComparison.OrdinalIgnoreCase))
        {
            throw new DatosLlegaronErradosException("Ingresá un correo diferente al actual.");
        }

        if (await _usuarioRepository.ExistePorEmail(emailNuevo))
        {
            throw new RecursoExistenteException("Ya existe una cuenta con ese correo.");
        }

        int? idTipo = await _tokenEmailRepository.ObtenerIdTipoPorNombre("CambioEmail");

        if (idTipo is null)
        {
            throw new InvalidOperationException("No está configurado el tipo CambioEmail.");
        }

        string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        string codigo = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        DateTime ahora = DateTime.UtcNow;

        string link = new UriBuilder(_settings.UrlVerificacion)
        {
            Query = $"token={token}&tipo=cambio-email",
            Fragment = string.Empty
        }.Uri.AbsoluteUri;

        string linkHtml = WebUtility.HtmlEncode(link);
        string contenidoHtml =
            "<h1>Confirmá tu nuevo correo</h1>" +
            "<p>Para completar el cambio de correo, abrí el siguiente link.</p>" +
            $"<p><a href=\"{linkHtml}\">Confirmar mi nuevo correo</a></p>" +
            "<p>El link vence en 30 minutos.</p>";

        TokenEmail tokenEmail = new TokenEmail
        {
            Codigo = codigo,
            FechaCreacion = ahora,
            FechaVencimiento = ahora.AddMinutes(30),
            EmailNuevo = emailNuevo,
            Usado = false,
            IdTipoTokenEmail = idTipo.Value,
            IdUsuario = idUsuario
        };

        await _tokenEmailRepository.GuardarAsync(tokenEmail);

        await _emailService.EnviarAsync(emailNuevo, "Confirmá tu nuevo correo en TiendaCry", contenidoHtml);

        _logger.LogInformation("Se envió un correo para confirmar el cambio de email del usuario {IdUsuario}.", idUsuario);
    }

    public async Task ConfirmarCambioEmail(ConfirmarEmailRequest request)
    {
        if (!Validaciones.EstanDatosBien(request.Token) || request.Token.Length != 64)
        {
            throw new DatosLlegaronErradosException("El link es inválido, venció o ya fue utilizado.");
        }

        string codigo = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Token)));

        bool confirmado = await _tokenEmailRepository.ConfirmarCambioEmail(codigo, DateTime.UtcNow);

        if (!confirmado)
        {
            throw new DatosLlegaronErradosException("El link es inválido, venció o ya fue utilizado.");
        }

        _logger.LogInformation("Se confirmó correctamente un cambio de correo electrónico.");
    }

    private async Task<Usuario> ObtenerUsuarioHabilitado(int idUsuario)
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

        return usuario;
    }
}
