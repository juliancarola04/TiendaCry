using Microsoft.Extensions.Options;
using System.Net;
using TiendaCry.Backend.Options;
using System.Security.Cryptography;
using System.Text;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Backend.Utilidades;

namespace TiendaCry.Backend.Services
{
    public class VerificacionEmailService
    {
        private readonly ITokenEmailRepository _tokenEmailRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly EmailService _emailService;
        private readonly EmailSettings _settings;

        public VerificacionEmailService(
            ITokenEmailRepository tokenEmailRepository,
            IUsuarioRepository usuarioRepository,
            EmailService emailService,
            IOptions<EmailSettings> settings)
        {
            _tokenEmailRepository = tokenEmailRepository;
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
            _settings = settings.Value;
        }

        public async Task EnviarVerificacionAsync(int idUsuario)
        {
            var usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            var token = await GenerarTokenAsync(idUsuario);

            var link = new UriBuilder(_settings.UrlVerificacion)
            {
                Query = $"token={token}",
                Fragment = string.Empty
            }.Uri.AbsoluteUri;

            var linkHtml = WebUtility.HtmlEncode(link);

            var contenidoHtml =
                "<h1>Verificá tu correo</h1>" +
                "<p>Para confirmar tu dirección, abrí el siguiente link.</p>" +
                $"<p><a href=\"{linkHtml}\">Verificar mi correo</a></p>" +
                "<p>El link vence en 30 minutos.</p>";

            await _emailService.EnviarAsync(
                usuario.Email,
                "Verificá tu correo en TiendaCry",
                contenidoHtml);
        }

        public async Task ConfirmarEmailAsync(string token)
        {
            if (!Validaciones.EstanDatosBien(token) || token.Length != 64)
            {
                throw new DatosLlegaronErradosException(
                    "El link de verificación es inválido, venció o ya fue utilizado.");
            }

            var codigo = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(token)));

            var confirmado = await _tokenEmailRepository.ConfirmarEmailAsync(
                codigo,
                DateTime.UtcNow);

            if (!confirmado)
            {
                throw new DatosLlegaronErradosException(
                    "El link de verificación es inválido, venció o ya fue utilizado.");
            }
        }

        public async Task<string> GenerarTokenAsync(int idUsuario)
        {
            var usuario = await _usuarioRepository.BuscarPorId(idUsuario);

            if (usuario is null)
            {
                throw new CredencialesInvalidasException();
            }

            if (usuario.EmailVerificado)
            {
                throw new DatosLlegaronErradosException(
                    "El correo electrónico ya está verificado.");
            }

            var idTipo = await _tokenEmailRepository
                .ObtenerIdTipoPorNombre("VerificacionEmail");

            if (idTipo is null)
            {
                throw new InvalidOperationException(
                    "No está configurado el tipo VerificacionEmail.");
            }

            var token = Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32));

            var codigo = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(token)));

            var ahora = DateTime.UtcNow;

            var tokenEmail = new TokenEmail
            {
                Codigo = codigo,
                FechaCreacion = ahora,
                FechaVencimiento = ahora.AddMinutes(30),
                EmailNuevo = null,
                Usado = false,
                IdTipoTokenEmail = idTipo.Value,
                IdUsuario = usuario.Id
            };

            await _tokenEmailRepository.GuardarAsync(tokenEmail);

            return token;
        }
    }
}
