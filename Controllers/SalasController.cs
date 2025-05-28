using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class SalasController : ControllerBase
    {
        private static List<Salas> Salas = new List<Salas>();

        private readonly ISalasService _serviceSalas;

        public SalasController(ISalasService service)
        {
            _serviceSalas = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Salas>>> GetSalas()
        {
            var Salas = await _serviceSalas.GetAllAsync();
            return Ok(Salas);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Salas>> GetSala(int id)
        {
            var sala = await _serviceSalas.GetByIdAsync(id);
            if (sala == null)
            {
                return NotFound();
            }
            return Ok(sala);
        }


        [HttpPost]
        public async Task<ActionResult<SalasDTO>> CreateSala(SalasDTO Salas)
        {
            await _serviceSalas.AddAsync(Salas);
            return CreatedAtAction(nameof(CreateSala), new { id = Salas.IdSala }, Salas);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSala(int id, Salas updatedSalas)
        {
            var existingSala = await _serviceSalas.GetByIdAsync(id);
            if (existingSala == null)
            {
                return NotFound();
            }

            existingSala.Nombre = updatedSalas.Nombre;
            existingSala.URL_Imagen = updatedSalas.URL_Imagen;
            existingSala.Capacidad = updatedSalas.Capacidad;
            existingSala.IdTipoSala = updatedSalas.IdTipoSala;
            existingSala.IdSede = updatedSalas.IdSede;
            existingSala.Bloqueado = updatedSalas.Bloqueado;


            await _serviceSalas.UpdateAsync(existingSala);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSala(int id)
        {
            var sala = await _serviceSalas.GetByIdAsync(id);
            if (sala == null)
            {
                return NotFound();
            }
            await _serviceSalas.DeleteAsync(id);
            return NoContent();
        }


        //  EJEMPLO https://localhost:7179/api/Salas/search?idsede=4
        [HttpGet("search")]
        public async Task<ActionResult<List<Salas>>> GetSalasBySede([FromQuery] int idsede)
        {
            var salas = await _serviceSalas.GetByIdSedeAsync(idsede);
            if (salas == null)
            {
                return NotFound("No se encontraron salas para la sede con ese ID");
            }
            return Ok(salas);
        }

        // https://localhost:7179/api/salas/getsalasdisponibles?idSede=1%20&fechaInicio=2024-05-20&fechaFin=2025-05-22&horaInicio=08:00:00&horaFin=18:00:00 ejemplo 
        [HttpGet("getsalasdisponibles")]
        public async Task<ActionResult<List<SalasFiltradoDTO>>> GetDisponiblesEnSede(
        // [FromQuery] son parametros que irán en URL
        /*si se usa el swagger para probar este get, el campo horaInicio y horaFin deben escribirse "08:00:00" (comillas incluidas), formato HH:mm:ss */
        [FromQuery] int idSede,
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin,
        [FromQuery] TimeSpan horaInicio,
        [FromQuery] TimeSpan horaFin)
        {
            var salas = await _serviceSalas.GetSalasBySede(idSede, fechaInicio, fechaFin, horaInicio, horaFin);
            if (salas == null)
            {
                return NotFound("No se encontraron salas disponibles en la sede especificada para la fecha dada");
            }

            return Ok(salas);
        }

        [HttpGet("con-caracteristicas")]
        public async Task<ActionResult<List<SalasConCaracteristicasDTO>>> GetAllWithCaracteristicas()
        {
            try
            {
                var salas = await _serviceSalas.GetAllWithCaracteristicasAsync();
                return Ok(salas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("{idSala}/caracteristicas/{idCaracteristica}")]
        public async Task<ActionResult> AddCaracteristicaToSala(int idSala, int idCaracteristica)
        {
            try
            {
                await _serviceSalas.AddCaracteristicaToSalaAsync(idSala, idCaracteristica);
                return Ok(new { message = $"Característica {idCaracteristica} agregada a la sala {idSala} exitosamente" });
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
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpDelete("{idSala}/caracteristicas/{idCaracteristica}")]
        public async Task<ActionResult> RemoveCaracteristicaFromSala(int idSala, int idCaracteristica)
        {
            try
            {
                await _serviceSalas.RemoveCaracteristicaFromSalaAsync(idSala, idCaracteristica);
                return Ok(new { message = $"Característica {idCaracteristica} eliminada de la sala {idSala} exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{idSala}/caracteristicas")]
        public async Task<ActionResult<List<CaracteristicaSalaDTO>>> GetCaracteristicasBySala(int idSala)
        {
            try
            {
                var caracteristicas = await _serviceSalas.GetCaracteristicasBySalaAsync(idSala);
                return Ok(caracteristicas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("puesto/{idPuestoTrabajo}/sala-nombre")] // EJ: https://localhost:7179/api/Salas/puesto/15/sala-nombre
        public async Task<ActionResult<string>> GetSalaNameByPuestoTrabajo(int idPuestoTrabajo)
        {
            try
            {
                var salaName = await _serviceSalas.GetSalaNameByPuestoTrabajoIdAsync(idPuestoTrabajo);

                if (salaName == null)
                {
                    return NotFound($"No se encontró el nombre de la sala para el puesto de trabajo con ID {idPuestoTrabajo}.");
                }

                return Ok(salaName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error del servidor: {ex.Message}");
            }
        }

    [HttpGet("puesto/{idPuestoTrabajo}/asiento-precio")] // EJ: https://localhost:7179/api/Salas/puesto/15/asiento-precio
            public async Task<ActionResult<decimal>> GetPrecioPuestoTrabajoAsync(int idPuestoTrabajo)
            {
                try
                {
                    var salaName = await _serviceSalas.GetPrecioPuestoTrabajoAsync(idPuestoTrabajo);

                    if (salaName == null)
                    {
                        return NotFound($"No se encontró el precio final del asiento con id {idPuestoTrabajo}.");
                    }

                    return Ok(salaName);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error del servidor: {ex.Message}");
                }
            }


    }

}