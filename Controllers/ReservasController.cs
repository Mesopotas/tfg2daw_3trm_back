using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;
using Models.DTOs;

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

    }
}