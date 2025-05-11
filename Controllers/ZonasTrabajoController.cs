using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class ZonasTrabajoController : ControllerBase
    {
        private static List<ZonasTrabajo> ZonasTrabajo = new List<ZonasTrabajo>();

        private readonly IZonasTrabajoService _serviceZonasTrabajo;

        public ZonasTrabajoController(IZonasTrabajoService service)
        {
            _serviceZonasTrabajo = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<ZonasTrabajo>>> GetZonasTrabajo()
        {
            var ZonasTrabajo = await _serviceZonasTrabajo.GetAllAsync();
            return Ok(ZonasTrabajo);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ZonasTrabajo>> GetZonaTrabajo(int id)
        {
            var zonaTrabajo = await _serviceZonasTrabajo.GetByIdAsync(id);
            if (zonaTrabajo == null)
            {
                return NotFound();
            }
            return Ok(zonaTrabajo);
        }


        [HttpPost]
        public async Task<ActionResult<ZonasTrabajoDTO>> CreateZonaTrabajo(ZonasTrabajoDTO ZonasTrabajo)
        {
            await _serviceZonasTrabajo.AddAsync(ZonasTrabajo);
            return CreatedAtAction(nameof(CreateZonaTrabajo), new { id = ZonasTrabajo.IdZonaTrabajo }, ZonasTrabajo);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateZonaTrabajo(int id, ZonasTrabajo updatedZonasTrabajo)
        {
            var existingZonaTrabajo = await _serviceZonasTrabajo.GetByIdAsync(id);
            if (existingZonaTrabajo == null)
            {
                return NotFound();
            }

            existingZonaTrabajo.Descripcion = updatedZonasTrabajo.Descripcion;


            await _serviceZonasTrabajo.UpdateAsync(existingZonaTrabajo);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteZonaTrabajo(int id)
        {
            var zonaTrabajo = await _serviceZonasTrabajo.GetByIdAsync(id);
            if (zonaTrabajo == null)
            {
                return NotFound();
            }
            await _serviceZonasTrabajo.DeleteAsync(id);
            return NoContent();
        }

    }
}