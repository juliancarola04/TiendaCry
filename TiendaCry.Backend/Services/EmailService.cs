using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TiendaCry.Backend.Options;

namespace TiendaCry.Backend.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task EnviarAsync(
            string destinatario,
            string asunto,
            string contenidoHtml)
        {
            var mensaje = new MimeMessage();

            mensaje.From.Add(new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = asunto;

            var contenido = new BodyBuilder
            {
                HtmlBody = contenidoHtml
            };

            mensaje.Body = contenido.ToMessageBody();

            using var cliente = new SmtpClient();

            await cliente.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await cliente.AuthenticateAsync(
                _settings.Username,
                _settings.Password);

            await cliente.SendAsync(mensaje);

            await cliente.DisconnectAsync(true);
        }
    }
}
