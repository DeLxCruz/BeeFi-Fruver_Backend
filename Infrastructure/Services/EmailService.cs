using Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services
{
    public class EmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger) : IEmailService
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task SendConfirmationEmailAsync(
            string email,
            string firstName,
            string confirmationLink)
        {
            var subject = "Confirma tu cuenta BeeFi";
            var htmlBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                    .content {{ background: #f9f9f9; padding: 30px; }}
                    .button {{ display: inline-block; padding: 15px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                    .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>¡Bienvenido a BeeFi! 🐝</h1>
                    </div>
                    <div class='content'>
                        <h2>Hola {firstName},</h2>
                        <p>Gracias por registrarte en BeeFi, tu plataforma de fruvers favorita.</p>
                        <p>Para completar tu registro, por favor confirma tu correo electrónico haciendo clic en el siguiente botón:</p>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='{confirmationLink}' class='button'>Confirmar Email</a>
                        </p>
                        <p>Si no creaste esta cuenta, puedes ignorar este correo.</p>
                        <p>¡Que disfrutes de la mejor experiencia comprando productos frescos!</p>
                        <p><strong>El equipo de BeeFi</strong></p>
                    </div>
                    <div class='footer'>
                        <p>Este es un correo automático, por favor no respondas.</p>
                        <p>© 2025 BeeFi. Todos los derechos reservados.</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendPasswordResetEmailAsync(
            string email,
            string firstName,
            string resetLink)
        {
            var subject = "Restablece tu contraseña - BeeFi";
            var htmlBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                    .content {{ background: #f9f9f9; padding: 30px; }}
                    .button {{ display: inline-block; padding: 15px 30px; background: #f5576c; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                    .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                    .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Restablecimiento de Contraseña 🔒</h1>
                    </div>
                    <div class='content'>
                        <h2>Hola {firstName},</h2>
                        <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta BeeFi.</p>
                        <p>Haz clic en el siguiente botón para crear una nueva contraseña:</p>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='{resetLink}' class='button'>Restablecer Contraseña</a>
                        </p>
                        <div class='warning'>
                            <strong>⚠️ Importante:</strong> Este enlace expirará en 1 hora por motivos de seguridad.
                        </div>
                        <p>Si no solicitaste este cambio, ignora este correo. Tu contraseña seguirá siendo la misma.</p>
                        <p><strong>El equipo de BeeFi</strong></p>
                    </div>
                    <div class='footer'>
                        <p>Este es un correo automático, por favor no respondas.</p>
                        <p>© 2025 BeeFi. Todos los derechos reservados.</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendOrderConfirmationEmailAsync(string email, string orderNumber)
        {
            var subject = $"Pedido Confirmado #{orderNumber} - BeeFi";
            var htmlBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                    .content {{ background: #f9f9f9; padding: 30px; }}
                    .order-box {{ background: white; border: 2px solid #4facfe; border-radius: 10px; padding: 20px; margin: 20px 0; text-align: center; }}
                    .order-number {{ font-size: 28px; font-weight: bold; color: #4facfe; }}
                    .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>¡Pedido Confirmado! ✅</h1>
                    </div>
                    <div class='content'>
                        <p>Tu pedido ha sido confirmado y está en proceso.</p>
                        <div class='order-box'>
                            <p style='margin: 0; color: #666;'>Número de Pedido</p>
                            <p class='order-number'>{orderNumber}</p>
                        </div>
                        <p>Te notificaremos cuando tu pedido esté en camino.</p>
                        <p>Puedes seguir el estado de tu pedido en tiempo real desde la aplicación.</p>
                        <p><strong>Gracias por tu compra! 🛒</strong></p>
                        <p><strong>El equipo de BeeFi</strong></p>
                    </div>
                    <div class='footer'>
                        <p>© 2025 BeeFi. Todos los derechos reservados.</p>
                    </div>
                </div>
            </body>
            </html>";

            await SendEmailAsync(email, subject, htmlBody);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress(string.Empty, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.SmtpPort,
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation(
                    "Email enviado exitosamente a {Email} con asunto: {Subject}",
                    toEmail,
                    subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al enviar email a {Email} con asunto: {Subject}",
                    toEmail,
                    subject);

                // No lanzar excepción para no bloquear el flujo principal
                // En producción podrías usar un sistema de cola (Hangfire, RabbitMQ)
            }
        }
    }
}