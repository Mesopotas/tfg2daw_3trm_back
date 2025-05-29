using Microsoft.AspNetCore.Mvc;
using CoWorking.DTO;
using CoWorking.Service;

namespace CoWorking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

   
        [HttpPost("contact-form")] // /api/Email/contact-form
        public async Task<IActionResult> SendContactForm([FromBody] EmailFormularioContactoDTO formData)
          {
            try
            {
                // une el nombre y apellidos para formar el nombre completo del remitente
                string senderFullName = $"{formData.Nombre} {formData.Apellidos}";
                // crea el asunto del correo con el nombre del remitente
                string emailSubject = $"Consulta de {senderFullName}";

                // llama al metodo del servicio para mandar el correo
                await _emailService.MandarCorreoFormulario(
                    senderFullName,
                    formData.Correo,
                    emailSubject,
                    formData.Consulta
                );

                // responde con exito si todo sale bien
                return Ok(new { message = "¡Mensaje enviado con exito! Te contactaremos pronto." });
            }
            catch (Exception ex)
            {
                // si hay un error devuelve estado 500 y un mensaje de error
                return StatusCode(500, new { message = "Error interno del servidor al enviar el mensaje. Por favor, intentalo de nuevo mas tarde." });
            }
        }
    }
}