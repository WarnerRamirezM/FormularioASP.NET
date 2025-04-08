using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string mensaje)
        {
            var clienteSmtp = new SmtpClient(_emailSettings.SmtpServer)
            {
                Port = _emailSettings.SmtpPort,
                Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true
            };

            mailMessage.To.Add(destinatario);

            await clienteSmtp.SendMailAsync(mailMessage);
        }
    }
}
