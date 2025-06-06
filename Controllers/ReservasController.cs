using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;
using Models.DTOs;
using System.Text.Json;
using QRCoder;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private static List<Reservas> detalleReservas = new List<Reservas>();

        private readonly IReservasService _serviceReservas;

        public ReservasController(IReservasService service)
        {
            _serviceReservas = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Reservas>>> GetReservas()
        {
            var reservas = await _serviceReservas.GetAllAsync();
            return Ok(reservas);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Reservas>> GetReserva(int id)
        {
            var detalleReservas = await _serviceReservas.GetByIdAsync(id);
            if (detalleReservas == null)
            {
                return NotFound();
            }
            return Ok(detalleReservas);
        }


        [HttpPost]
        public async Task<ActionResult<ReservasDTO>> CreateReserva(Reservas reservas)
        {
            await _serviceReservas.CreateReservaAsync(reservas);
            return CreatedAtAction(nameof(CreateReserva), new { id = reservas.IdReserva }, reservas);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReserva(int id, ReservasUpdateDTO updatedReservas)
        {


            try
            {
                await _serviceReservas.UpdateAsync(updatedReservas);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la reserva: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var detalleReservas = await _serviceReservas.GetByIdAsync(id);
            if (detalleReservas == null)
            {
                return NotFound();
            }
            await _serviceReservas.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("reservasdeusuario/{idUsuario}/")]
        public async Task<ActionResult<GetReservasClienteDTO>> GetDetallesPedido(int idUsuario)
        {
            var reservaDetalles = await _serviceReservas.GetReservasUsuario(idUsuario);
            if (reservaDetalles == null)
            {
                return NotFound();
            }
            return Ok(reservaDetalles);
        }

[HttpGet("resumencompra/{id}")]
public async Task<ActionResult<GetDetallesReservaDTO>> GetResumenReserva(int id)
{
    try
    {
        var resumenReserva = await _serviceReservas.GetResumenReservaAsync(id);
        
        if (resumenReserva == null)
        {
            return NotFound($"No se encontró la reserva con ID {id}");
        }
        
        return Ok(resumenReserva);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al obtener el resumen de la reserva: {ex.Message}");
    }
}
        /*
        EJEMPLO BODY PARA EL ENDPOINT (todos los IDs deben existir en la base de datos)
        {
          "idUsuario": 1,
          "descripcion": "Reserva",
          "fechaReserva": "2025-05-14T11:30:00",
          "lineas": [
            {
              "idPuestoTrabajo": 1,
              "idTramoHorario": 2
            },
            {
              "idPuestoTrabajo": 2,
              "idTramoHorario": 2
            },
          {
              "idPuestoTrabajo": 3,
              "idTramoHorario": 2
            }
          ]
        }*/
        [HttpPost("reservacompleta")]
        public async Task<ActionResult<Reservas>> CrearReservaConLineas([FromBody] ReservaPostDTO reservaDTO) // [FromBody] hace que se explique que debe haber un contenido en el BODY de la peticion y no ir todo por path
        {
            try
            {
                var reservaCreada = await _serviceReservas.CreateReservaConLineasAsync(reservaDTO);
                return CreatedAtAction(nameof(GetReserva), new { id = reservaCreada.IdReserva }, reservaCreada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("generarqr/{id}")] // GET https://localhost:7179/api/Reservas/generarqr/1 
        [Produces("image/png")] // el endpoint respondera un png
        public async Task<IActionResult> GenerarQr(int id)
        {
            // obtener la info de la reserva
            var reservaDTO = await _serviceReservas.GetByIdAsync(id);

            if (reservaDTO == null)
            {
                return NotFound("Reserva no encontrada para generar el QR.");
            }

            // construir el enlace al que deberá apuntar el QR, con los campos a validar siendo variables obtenidad del getbyid
            string urlEndpointValidador = $"https://coworkingapi.jblas.me/api/Reservas/validarReservaQR" +
                                   $"?idReserva={reservaDTO.IdReserva}" +
                                   $"&idUsuario={reservaDTO.UsuarioId}" +
                                   $"&fecha={reservaDTO.Fecha.ToString("yyyy-MM-dd")}"; // formato que el endpoint acepta

            byte[] qrCodeBytes;
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(urlEndpointValidador, QRCodeGenerator.ECCLevel.Q)) // genera el qr de la url con los campos variables
            {
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    qrCodeBytes = qrCode.GetGraphic(5); // tamaño del qr autoajustado
                }
            }
            return File(qrCodeBytes, "image/png"); // respuesta png
        }

        

        // ejemplo: https://localhost:7179/api/Reservas/validarReservaQR?idReserva=2&idUsuario=2&fecha=2025-05-23
         [HttpGet("validarReservaQR")]
        [Produces("text/plain")] // Será una comprobacion normal, asi que la respuesta será un texto plano en vez de un json
        public async Task<IActionResult> ValidarQrSimple(
            // parametros de query los 3 valores
            [FromQuery] int idReserva,
            [FromQuery] int idUsuario,
            [FromQuery] DateTime fecha)
        {
            bool existe = await _serviceReservas.ValidarReservaExisteQR(idReserva, idUsuario, fecha);

            if (existe)
            {
                return Ok("Si, la reserva existe"); // deuvelve "Si" si la reserva es valida
            }
            else
            {
                return Ok("Esta reserva no existe");
            }
        }
    }
}