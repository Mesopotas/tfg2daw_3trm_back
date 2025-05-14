using Microsoft.AspNetCore.Mvc;
using CoWorking.Service;
using CoWorking.DTO;
using Dtos;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuestosTrabajoController : ControllerBase
    {
        private readonly IPuestosTrabajoService _servicePuestosTrabajo;

        public PuestosTrabajoController(IPuestosTrabajoService service)
        {
            _servicePuestosTrabajo = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<PuestosTrabajoDTO>>> GetPuestosTrabajo()
        {
            var puestos = await _servicePuestosTrabajo.GetAllAsync();
            return Ok(puestos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PuestosTrabajoDTO>> GetPuestoTrabajoById(int id)
        {
            var puesto = await _servicePuestosTrabajo.GetByIdAsync(id);
            if (puesto == null)
                return NotFound();

            return Ok(puesto);
        }

        [HttpPost]
        public async Task<ActionResult<PuestosTrabajoDTO>> CreatePuestoTrabajo(PuestosTrabajoDTO nuevoPuesto)
        {
            await _servicePuestosTrabajo.AddAsync(nuevoPuesto);
            return CreatedAtAction(nameof(GetPuestoTrabajoById), new { id = nuevoPuesto.IdPuestoTrabajo }, nuevoPuesto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePuestoTrabajo(int id, PuestosTrabajoDTO puestoActualizado)
        {
            var existente = await _servicePuestosTrabajo.GetByIdAsync(id);
            if (existente == null)
                return NotFound();

            puestoActualizado.IdPuestoTrabajo = id;

            await _servicePuestosTrabajo.UpdateAsync(puestoActualizado);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePuestoTrabajo(int id)
        {
            var existente = await _servicePuestosTrabajo.GetByIdAsync(id);
            if (existente == null)
                return NotFound();

            await _servicePuestosTrabajo.DeleteAsync(id);
            return NoContent();
        }

    // ejemplo: https://localhost:7179/api/puestosTrabajo/disponibles?idSede=1&fechaInicio=2025-06-01&fechaFin=2025-06-10&horaInicio=08:00:00&horaFin=17:00:00

         [HttpGet("disponibles")]
    public async Task<ActionResult<List<PuestosTrabajoDTO>>> GetDisponiblesEnSede(
        // [FromQuery] son parametros que irán en URL
        /*si se usa el swagger para probar este get, el campo horaInicio y horaFin deben escribirse "08:00:00" (comillas incluidas), formato HH:mm:ss */
        [FromQuery] int idSede, 
        [FromQuery] DateTime fechaInicio, 
        [FromQuery] DateTime fechaFin, 
        [FromQuery] TimeSpan horaInicio, 
        [FromQuery] TimeSpan horaFin)
    {
        var puestosDisponibles = await _servicePuestosTrabajo.GetDisponiblesEnSedeAsync(idSede, fechaInicio, fechaFin, horaInicio, horaFin);
        if (puestosDisponibles == null)
        {
            return NotFound("No se encontraron puestos disponibles en la sede especificada para la fecha dada");
        }

        return Ok(puestosDisponibles);
    }
    }
}
