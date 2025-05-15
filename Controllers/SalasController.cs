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

    }
/*
    public class SalasController : ControllerBase
    {
        private static List<Salas> sala = new List<Salas>();

        private readonly ISalasService _serviceSalas;

        public SalasController(ISalasService service)
        {
            _serviceSalas = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Salas>>> GetSalas()
        {
            var salas = await _serviceSalas.GetAllAsync();
            return Ok(salas);
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

[HttpGet("search")]
public async Task<ActionResult<List<SalasDTO>>> GetSalaBySede([FromQuery] int idsede)
{
    var salas = await _serviceSalas.GetByIdSedeAsync(idsede);

    if (salas == null || !salas.Any())
    {
        return NotFound("No se encontraron salas para la sede de ese id");
    }

    return Ok(salas);
}
        [HttpPost]
        public async Task<ActionResult<Salas>> CreateSala(Salas salas)
        {
            await _serviceSalas.AddAsync(salas);
            return CreatedAtAction(nameof(CreateSala), new { id = salas.IdSala }, salas);
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
                    existingSala.Capacidad = updatedSalas.Capacidad;
                    existingSala.IdTipoSala = updatedSalas.IdTipoSala;
                    existingSala.IdSede = updatedSalas.IdSede;


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
    } */
}