using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SedesController : ControllerBase
    {
        private static List<Sedes> sede = new List<Sedes>();

        private readonly ISedesService _serviceSedes;

        public SedesController(ISedesService service)
        {
            _serviceSedes = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Sedes>>> GetSedes()
        {
            var sedes = await _serviceSedes.GetAllAsync();
            return Ok(sedes);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Sedes>> GetSede(int id)
        {
            var sede = await _serviceSedes.GetByIdAsync(id);
            if (sede == null)
            {
                return NotFound();
            }
            return Ok(sede);
        }


        [HttpPost]
        public async Task<ActionResult<Sedes>> CreateSede(Sedes sedes)
        {
            await _serviceSedes.AddAsync(sedes);
            return CreatedAtAction(nameof(CreateSede), new { id = sedes.IdSede }, sedes);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSede(int id, Sedes updatedSedes)
        {
            var existingSede = await _serviceSedes.GetByIdAsync(id);
            if (existingSede == null)
            {
                return NotFound();
            }
            existingSede.Pais = updatedSedes.Pais;
            existingSede.Ciudad = updatedSedes.Ciudad;
            existingSede.Direccion = updatedSedes.Direccion;
            existingSede.CodigoPostal = updatedSedes.CodigoPostal;
            existingSede.Planta = updatedSedes.Planta;
            existingSede.Observaciones = updatedSedes.Observaciones;


            await _serviceSedes.UpdateAsync(existingSede);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSede(int id)
        {
            var sede = await _serviceSedes.GetByIdAsync(id);
            if (sede == null)
            {
                return NotFound();
            }
            await _serviceSedes.DeleteAsync(id);
            return NoContent();
        }

    }
}