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
        private readonly IHttpClientFactory _httpClientFactory; // necesario para obtener y adjuntar el qr code

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendReservationConfirmationAsync(string toEmail, string userName, ReservationEmailData reservationData)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"],
                _configuration["EmailSettings:SenderEmail"])
            );
            message.To.Add(new MailboxAddress(userName, toEmail));
            message.Subject = $"Confirmación de Reserva #{reservationData.IdReserva}";

            var cuerpoEmail = new BodyBuilder();
            cuerpoEmail.HtmlBody = CreateEmailTemplate(userName, reservationData);

            string qrCodeApiUrl = $"https://localhost:7179/api/Reservas/generarqr/{reservationData.IdReserva}"; // endpoint que genera los qr
            byte[] qrCodeBytes = null;


            HttpResponseMessage response = await httpClient.GetAsync(qrCodeApiUrl);
            response.EnsureSuccessStatusCode();

            qrCodeBytes = await response.Content.ReadAsByteArrayAsync();

            if (qrCodeBytes != null && qrCodeBytes.Length > 0)
            {
                var attachment = new MimePart("image", "png")
                {
                    Content = new MimeContent(new MemoryStream(qrCodeBytes)),
                    ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = $"qr_reserva_{reservationData.IdReserva}.png" // nombre con id de reserva incluido
                };
                cuerpoEmail.Attachments.Add(attachment);
            }


            message.Body = cuerpoEmail.ToMessageBody();

            using var client = new SmtpClient();

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

        public async Task MandarCorreoFormulario(string fromName, string fromEmail, string subject, string messageBody)
        {

            var message = new MimeMessage();

            // correo de app settings, será a nosotros mismos
            message.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"],
                _configuration["EmailSettings:SenderEmail"])
            );

            // se enviara al correo del appsettings
            message.To.Add(new MailboxAddress(
                _configuration["EmailSettings:ContactFormRecipientName"] ?? "Admin CoWorking",
                _configuration["EmailSettings:ContactFormRecipientEmail"])
            );

            message.ReplyTo.Add(new MailboxAddress(fromName, fromEmail));

            message.Subject = $"Formulario de Contacto: {subject}";

            var cuerpoEmail = new BodyBuilder();
            cuerpoEmail.HtmlBody = $@"
                    <html>
                    <body>
                        <h2>Nuevo Mensaje del Formulario de Contacto</h2>
                        <p><strong>Nombre:</strong> {fromName}</p>
                        <p><strong>Email:</strong> {fromEmail}</p>
                        <p><strong>Asunto:</strong> {subject}</p>
                        <p><strong>Mensaje:</strong></p>
                        <p>{messageBody}</p>
                        <hr>
                        <p>Este mensaje fue enviado desde el formulario de contacto de tu sitio web.</p>
                    </body>
                    </html>";
            message.Body = cuerpoEmail.ToMessageBody();

            // config correo enviar
            using var client = new SmtpClient();

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
        
        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
{
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(
        _configuration["EmailSettings:SenderName"],
        _configuration["EmailSettings:SenderEmail"]
    ));
    message.To.Add(new MailboxAddress(userName, toEmail));
    message.Subject = "¡Bienvenido a CoWorking!";

    var cuerpo = new BodyBuilder();
    cuerpo.HtmlBody = CreateWelcomeTemplate(userName);
    message.Body = cuerpo.ToMessageBody();

    using var client = new SmtpClient();
    var host = _configuration["EmailSettings:SmtpHost"];
    var port = int.Parse(_configuration["EmailSettings:SmtpPort"]);
    var user = _configuration["EmailSettings:SmtpUsername"];
    var pass = _configuration["EmailSettings:SmtpPassword"];

    await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
    await client.AuthenticateAsync(user, pass);
    await client.SendAsync(message);
    await client.DisconnectAsync(true);
}

// generar el HTML del email
private string CreateWelcomeTemplate(string userName)
{
    return $@"<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <style>
    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
    .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
    .content {{ padding: 20px; background-color: #f9f9f9; }}
    .footer {{ text-align: center; padding: 15px; color: #666; font-size: 0.9em; }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <h1>¡Bienvenido, {userName}!</h1>
    </div>
    <div class='content'>
      <p>Gracias por registrarte en <strong>CoWorking</strong>. Estamos muy contentos de tenerte con nosotros.</p>
      <p>A continuación algunos pasos para comenzar:</p>
      <ul>
        <li>Explora nuestros espacios disponibles.</li>
        <li>Reserva tu primer puesto o sala de reuniones.</li>
        <li>Contacta con soporte si tienes cualquier duda.</li>
      </ul>
      <p>¡Esperamos verte pronto!</p>
    </div>
    <div class='footer'>
      <p>Este es un correo automático, por favor no respondas.</p>
      <p>&copy; 2025 CoWorking.</p>
    </div>
  </div>
</body>
</html>";
}

    }
}