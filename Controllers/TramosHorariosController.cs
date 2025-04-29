using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TramosHorariosController : ControllerBase
    {
        private static List<TramosHorarios> tramosHorarioss = new List<TramosHorarios>();

        private readonly ITramosHorariosService _serviceTramosHorarios;

        public TramosHorariosController(ITramosHorariosService service)
        {
            _serviceTramosHorarios = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TramosHorarios>>> GetTramosHorarios()
        {
            var tramosHorarioss = await _serviceTramosHorarios.GetAllAsync();
            return Ok(tramosHorarioss);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TramosHorarios>> GetTramosHorario(int id)
        {
            var tramosHorarios = await _serviceTramosHorarios.GetByIdAsync(id);
            if (tramosHorarios == null)
            {
                return NotFound();
            }
            return Ok(tramosHorarios);
        }


        [HttpPost]
        public async Task<ActionResult<TramosHorarios>> CreateTramosHorario(TramosHorarios tramosHorarioss)
        {
            await _serviceTramosHorarios.AddAsync(tramosHorarioss);
            return CreatedAtAction(nameof(CreateTramosHorario), new { id = tramosHorarioss.IdTramoHorario }, tramosHorarioss);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTramosHorario(int id, TramosHorarios updatedTramosHorarios)
        {
            var existingTramosHorario = await _serviceTramosHorarios.GetByIdAsync(id);
            if (existingTramosHorario == null)
            {
                return NotFound();
            }
            existingTramosHorario.HoraInicio = updatedTramosHorarios.HoraInicio;
            existingTramosHorario.HoraFin = updatedTramosHorarios.HoraFin;
            existingTramosHorario.DiaSemanal = updatedTramosHorarios.DiaSemanal;


            await _serviceTramosHorarios.UpdateAsync(existingTramosHorario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTramosHorario(int id)
        {
            var tramosHorarios = await _serviceTramosHorarios.GetByIdAsync(id);
            if (tramosHorarios == null)
            {
                return NotFound();
            }
            await _serviceTramosHorarios.DeleteAsync(id);
            return NoContent();
        }

    }
}