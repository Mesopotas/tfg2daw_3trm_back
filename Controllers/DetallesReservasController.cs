using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallesReservasController : ControllerBase
    {
        private static List<DetallesReservas> detalleReservas = new List<DetallesReservas>();

        private readonly IDetallesReservasService _serviceDetallesReservas;

        public DetallesReservasController(IDetallesReservasService service)
        {
            _serviceDetallesReservas = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DetallesReservas>>> GetDetallesReservas()
        {
            var detallesReservas = await _serviceDetallesReservas.GetAllAsync();
            return Ok(detallesReservas);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<DetallesReservas>> GetDetallesReserva(int id)
        {
            var detalleReservas = await _serviceDetallesReservas.GetByIdAsync(id);
            if (detalleReservas == null)
            {
                return NotFound();
            }
            return Ok(detalleReservas);
        }
    [HttpGet("last")]
            public async Task<ActionResult<int?>> GetLastIdDetallesReserva()
            {
                var lastId = await _serviceDetallesReservas.GetLastIdAsync();
                if (lastId == null)
                {
                    return NotFound("No hay registros disponibles.");
                }
                return Ok(lastId);
            }

        [HttpPost]
        public async Task<ActionResult<DetallesReservas>> CreateDetallesReserva(DetallesReservas detallesReservas)
        {
            await _serviceDetallesReservas.AddAsync(detallesReservas);
            return CreatedAtAction(nameof(CreateDetallesReserva), new { id = detallesReservas.IdDetalleReserva }, detallesReservas);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDetallesReserva(int id, DetallesReservas updatedDetallesReservas)
        {
            var existingDetallesReserva = await _serviceDetallesReservas.GetByIdAsync(id);
            if (existingDetallesReserva == null)
            {
                return NotFound();
            }
            existingDetallesReserva.Descripcion = updatedDetallesReservas.Descripcion;


            await _serviceDetallesReservas.UpdateAsync(existingDetallesReserva);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetallesReserva(int id)
        {
            var detalleReservas = await _serviceDetallesReservas.GetByIdAsync(id);
            if (detalleReservas == null)
            {
                return NotFound();
            }
            await _serviceDetallesReservas.DeleteAsync(id);
            return NoContent();
        }

    }
}