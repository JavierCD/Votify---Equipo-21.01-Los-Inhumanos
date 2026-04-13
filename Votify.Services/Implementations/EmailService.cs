using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Votify.Services.Interfaces;

namespace Votify.Services.Implementations
{
    public class EmailService : IEmailService
    {
        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                // CONFIGURACIÓN SMTP (Para producción: usar Mailtrap, SendGrid, Gmail...)
                string smtpServer = "smtp.tuserver.com"; // Cambiar por tu servidor
                int smtpPort = 587;
                string smtpUser = "tu_correo@dominio.com";
                string smtpPass = "tu_contraseña";

                // Si estás en local y no tienes SMTP configurado, simulamos el envío en consola
                if (smtpServer == "smtp.tuserver.com")
                {
                    Console.WriteLine($"\n[MOCK EMAIL ENVIADO A: {destinatario}]");
                    Console.WriteLine($"ASUNTO: {asunto}");
                    Console.WriteLine($"CUERPO HTML:\n{cuerpo}\n");
                    return;
                }

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser, "Eventos Votify"),
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = true // ¡Importante para que se vea la tabla!
                };

                mailMessage.To.Add(destinatario);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando email a {destinatario}: {ex.Message}");
            }
        }
        
    }
}
