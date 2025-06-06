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
        public async Task<ActionResult<List<PuestoTrabajoFiltroFechasDTO>>> GetDisponiblesEnSede(
       // [FromQuery] son parametros que irán en URL
       /*si se usa el swagger para probar este get, el campo horaInicio y horaFin deben escribirse "08:00:00" (comillas incluidas), formato HH:mm:ss */
       [FromQuery] int idSala, // ahora irá con el id de la sala en vez de id de la sede
       [FromQuery] DateTime fechaInicio,
       [FromQuery] DateTime fechaFin,
       [FromQuery] TimeSpan horaInicio,
       [FromQuery] TimeSpan horaFin)
        {


            var puestosDisponibles = await _servicePuestosTrabajo.GetPuestosWithAvailabilityBySalaAsync(idSala, fechaInicio, fechaFin, horaInicio, horaFin);
            if (puestosDisponibles == null)
            {
                return NotFound("No se encontraron puestos disponibles en la sede especificada para la fecha dada");
            }

            return Ok(puestosDisponibles);
        }
    

    // igual que el de arriba pero en vez de una lista devolverá solo el primer objeto (asiento)
     [HttpGet("puestounicodisponible")] // EJ: https://localhost:7179/api/puestostrabajo/puestounicodisponible?idSala=1&fechaInicio=2025-06-05&fechaFin=2025-06-05&horaInicio=08%3A00&horaFin=19%3A00
        public async Task<ActionResult<PuestoTrabajoFiltroFechasDTO>> GetPuestoUnicoWithAvailabilityBySalaAsync(
       // [FromQuery] son parametros que irán en URL
       /*si se usa el swagger para probar este get, el campo horaInicio y horaFin deben escribirse "08:00:00" (comillas incluidas), formato HH:mm:ss */
       [FromQuery] int idSala, // ahora irá con el id de la sala en vez de id de la sede
       [FromQuery] DateTime fechaInicio,
       [FromQuery] DateTime fechaFin,
       [FromQuery] TimeSpan horaInicio,
       [FromQuery] TimeSpan horaFin)
        {


            var puestosDisponibles = await _servicePuestosTrabajo.GetPuestoUnicoWithAvailabilityBySalaAsync(idSala, fechaInicio, fechaFin, horaInicio, horaFin);
            if (puestosDisponibles == null)
            {
                return NotFound("No se encontraron puestos disponibles en la sede especificada para la fecha dada");
            }

            return Ok(puestosDisponibles);
        }

     [HttpPost("generarAsientosDeSalas")]
        public async Task<IActionResult> GenerarAsientosDeSalas()
        {
            try
            {
                await _servicePuestosTrabajo.GenerarAsientosDeSalas();
                return Ok("Asientos generados exitosamente para las salas que no tenían.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar asientos para las salas: {ex.Message}");
                return StatusCode(500, $"Error del servidor al generar asientos para las salas: {ex.Message}");
            }
        }
    }
    }

