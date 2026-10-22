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

namespace TiendaCry.Backend.Services;

public class RecuperacionContrasenaService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenEmailRepository _tokenEmailRepository;
    private readonly EmailService _emailService;
    private readonly EmailSettings _settings;

    public RecuperacionContrasenaService(IUsuarioRepository usuarioRepository, ITokenEmailRepository tokenEmailRepository, EmailService emailService, IOptions<EmailSettings> settings)
    {
        _usuarioRepository = usuarioRepository;
        _tokenEmailRepository = tokenEmailRepository;
        _emailService = emailService;
        _settings = settings.Value;
    }

    public async Task RestablecerContrasena(RestablecerContrasenaRequest request)
    {
        if (!Validaciones.EstanDatosBien(request.Token, request.Contrasena))
        {
            throw new DatosLlegaronErradosException("Todos los campos son obligatorios.");
        }

        if (request.Token.Length != 64)
        {
            throw new DatosLlegaronErradosException("El link es inválido, venció o ya fue utilizado.");
        }

        string codigo = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Token)));
        string contrasena = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Contrasena);

        bool restablecida = await _tokenEmailRepository.RestablecerContrasena(codigo, contrasena, DateTime.UtcNow);

        if (!restablecida)
        {
            throw new DatosLlegaronErradosException("El link es inválido, venció o ya fue utilizado.");
        }
    }

    public async Task SolicitarRecuperacion(SolicitarRecuperacionContrasenaRequest request)
    {
        if (!Validaciones.EstanDatosBien(request.Email))
        {
            throw new DatosLlegaronErradosException("El correo es obligatorio.");
        }

        string email = request.Email.Trim();

        if (!Validaciones.CumpleLongitudMaxima(email, 254)
            || !Validaciones.EsUnEmailValido(email))
        {
            throw new DatosLlegaronErradosException("El formato del correo es inválido.");
        }

        Usuario? usuario = await _usuarioRepository.BuscarPorEmail(email);

        if (usuario is null
            || !usuario.EmailVerificado
            || usuario.EstadoUsuario.Nombre != "Activo")
        {
            return;
        }

        int? idTipo = await _tokenEmailRepository.ObtenerIdTipoPorNombre("RecuperacionContrasena");

        if (idTipo is null)
        {
            throw new InvalidOperationException(
                "No está configurado el tipo de token RecuperacionContrasena.");
        }

        string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        string codigo = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        DateTime ahora = DateTime.UtcNow;

        TokenEmail tokenEmail = new TokenEmail
        {
            Codigo = codigo,
            FechaCreacion = ahora,
            FechaVencimiento = ahora.AddMinutes(30),
            EmailNuevo = null,
            Usado = false,
            IdTipoTokenEmail = idTipo.Value,
            IdUsuario = usuario.Id
        };

        string link = new UriBuilder(_settings.UrlRecuperacionContrasena)
        {
            Query = $"token={token}",
            Fragment = string.Empty
        }.Uri.AbsoluteUri;

        string linkHtml = WebUtility.HtmlEncode(link);

        string contenidoHtml =
            "<h1>Restablecé tu contraseña</h1>" +
            "<p>Para elegir una nueva contraseña, abrí el siguiente link.</p>" +
            $"<p><a href=\"{linkHtml}\">Restablecer mi contraseña</a></p>" +
            "<p>El link vence en 30 minutos.</p>";

        await _tokenEmailRepository.GuardarAsync(tokenEmail);

        await _emailService.EnviarAsync(usuario.Email, "Restablecé tu contraseña en TiendaCry", contenidoHtml);
    }
}
