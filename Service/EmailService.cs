using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using CoWorking.Models.DTO;
using Coworking.Configs;

namespace CoWorking.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
        }

        public async Task SendReservationConfirmationAsync(string toEmail, string userName, ReservationEmailData reservationData)
        {
         
                var message = new MimeMessage();

                // toda la data saldrá del appsettings.json
                message.From.Add(new MailboxAddress(
                    _configuration["EmailSettings:SenderName"],
                    _configuration["EmailSettings:SenderEmail"])
                );
                message.To.Add(new MailboxAddress(userName, toEmail));
                message.Subject = $"Confirmación de Reserva #{reservationData.IdReserva}";

                // cuerpo del email
                var cuerpoEmail = new BodyBuilder();
                cuerpoEmail.HtmlBody = CreateEmailTemplate(userName, reservationData);
                message.Body = cuerpoEmail.ToMessageBody();

                // configurar y mandar email
                using var client = new SmtpClient();

                // configs autenticacion y conexion al server SMTP (gmail), tb se saca del appsettings.json
                string smtpHost = _configuration["EmailSettings:SmtpHost"];
                string smtpPortString = _configuration["EmailSettings:SmtpPort"];
                string smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                string smtpPassword = _configuration["EmailSettings:SmtpPassword"];

             

                int smtpPort = int.Parse(smtpPortString);

                await client.ConnectAsync(
                    smtpHost,
                    smtpPort,
                    SecureSocketOptions.StartTls
                );
                await client.AuthenticateAsync(
                    smtpUsername,
                    smtpPassword
                );
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

           
        }

        // metodo crear el email con formato HTML y todo
        private string CreateEmailTemplate(string userName, ReservationEmailData data)
        {
            return $@"<!DOCTYPE html><html><head>    <meta charset='utf-8'>    <style>        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}        .content {{ padding: 20px; background-color: #f9f9f9; }}        .details {{ background-color: white; padding: 15px; margin: 10px 0; border-radius: 5px; }}        .footer {{ text-align: center; padding: 20px; color: #666; }}        .success {{ color: #4CAF50; font-weight: bold; }}    </style></head><body>    <div class='container'>        <div class='header'>            <h1>¡Reserva Confirmada!</h1>        </div>

                <div class='content'>
            <h2>Hola {userName},</h2>
            <p class='success'>Tu reserva ha sido confirmada exitosamente.</p>

                        <div class='details'>
                <h3>Detalles de tu Reserva</h3>
                <p><strong>Número de Reserva:</strong> #{data.IdReserva}</p>
                <p><strong>Fecha de Reserva:</strong> {data.Fecha:dd/MM/yyyy}</p>
                <p><strong>Precio Total:</strong> {data.PrecioTotal:C}</p>

                                {(string.IsNullOrEmpty(data.NombreSala) ? "" : $"<p><strong>Sala:</strong> {data.NombreSala}</p>")}
                {(string.IsNullOrEmpty(data.CiudadSede) ? "" : $"<p><strong>Ubicación:</strong> {data.CiudadSede}</p>")}
                {(string.IsNullOrEmpty(data.DireccionSede) ? "" : $"<p><strong>Dirección:</strong> {data.DireccionSede}</p>")}
                {(string.IsNullOrEmpty(data.RangoHorario) ? "" : $"<p><strong>Horario:</strong> {data.RangoHorario}</p>")}
                {(string.IsNullOrEmpty(data.AsientosReservados) ? "" : $"<p><strong>Asientos:</strong> {data.AsientosReservados}</p>")}
            </div>

            <p>Gracias por elegir nuestro espacio de coworking. Te esperamos en la fecha programada.</p>
        </div>
            <p>Este es un email automático, por favor no respondas a este mensaje.</p>
        </div>
    </div></body></html>";
        }
    }
}