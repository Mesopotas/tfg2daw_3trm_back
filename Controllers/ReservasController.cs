using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

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


     /*   [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReserva(int id, Reservas updatedReservas)
        {
            var existingReserva = await _serviceReservas.GetByIdAsync(id);
            if (existingReserva == null)
            {
                return NotFound();
            }
            existingReserva.Descripcion = updatedReservas.Descripcion;


            await _serviceReservas.UpdateAsync(existingReserva);
            return NoContent();
        }
*/
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
[HttpGet("detalles/{idReserva}/{idDetalleReserva}")]
public async Task<ActionResult<ReservasClienteInfoDTO>> GetDetallesPedido(int idReserva, int idDetalleReserva)
{
    var reservaDetalles = await _serviceReservas.GetDetallesPedido(idReserva, idDetalleReserva);
    if (reservaDetalles == null)
    {
        return NotFound();
    }
    return Ok(reservaDetalles);
}

    }
}