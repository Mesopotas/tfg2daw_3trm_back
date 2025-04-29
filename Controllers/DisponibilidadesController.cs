using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisponibilidadesController : ControllerBase
    {
        private static List<DisponibilidadDTO> sala = new List<DisponibilidadDTO>();

        private readonly IDisponibilidadesService _serviceDisponibilidad;

        public DisponibilidadesController(IDisponibilidadesService service)
        {
            _serviceDisponibilidad = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DisponibilidadDTO>>> GetDisponibilidades()
        {
            var disponibilidades = await _serviceDisponibilidad.GetAllAsync();
            return Ok(disponibilidades);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<DisponibilidadDTO>> GetDisponibilidad(int id)
        {
            var sala = await _serviceDisponibilidad.GetByIdAsync(id);
            if (sala == null)
            {
                return NotFound();
            }
            return Ok(sala);
        }

        [HttpGet("search")] // ejemplo endpoint: https://localhost:7179/api/Disponibilidades/search?idPuestoTrabajo=18 
        public async Task<ActionResult<List<DisponibilidadDTO>>> GetSalaBySede([FromQuery] int idPuestoTrabajo)
        {
            var disponibilidades = await _serviceDisponibilidad.GetByIdPuestoTrabajoAsync(idPuestoTrabajo);

            if (disponibilidades == null || !disponibilidades.Any())
            {
                return NotFound("No se encontraron disponibilidades para el asiento de ese id");
            }

            return Ok(disponibilidades);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSala(int id, DisponibilidadDTO updatedDisponibilidad)
        {
            var existingDisponibilidad = await _serviceDisponibilidad.GetByIdAsync(id);
            if (existingDisponibilidad == null)
            {
                return NotFound();
            }
            existingDisponibilidad.Estado = updatedDisponibilidad.Estado;


            await _serviceDisponibilidad.UpdateDisponibilidadAsync(existingDisponibilidad);
            return NoContent();
        }

    }
}