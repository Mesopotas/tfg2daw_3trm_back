using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoSalasController : ControllerBase
    {
        private static List<TipoSalas> tipoSalas = new List<TipoSalas>();

        private readonly ITipoSalasService _serviceTipoSalas;

        public TipoSalasController(ITipoSalasService service)
        {
            _serviceTipoSalas = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TipoSalas>>> GetTipoSalas()
        {
            var tipoSalas = await _serviceTipoSalas.GetAllAsync();
            return Ok(tipoSalas);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TipoSalas>> GetTipoSala(int id)
        {
            var tipoSala = await _serviceTipoSalas.GetByIdAsync(id);
            if (tipoSala == null)
            {
                return NotFound();
            }
            return Ok(tipoSala);
        }


        [HttpPost]
        public async Task<ActionResult<TipoSalas>> CreateTipoSala(TipoSalas tipoSalas)
        {
            await _serviceTipoSalas.AddAsync(tipoSalas);
            return CreatedAtAction(nameof(CreateTipoSala), new { id = tipoSalas.IdTipoSala }, tipoSalas);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTipoSala(int id, TipoSalas updatedTipoSalas)
        {
            var existingTipoSala = await _serviceTipoSalas.GetByIdAsync(id);
            if (existingTipoSala == null)
            {
                return NotFound();
            }
            existingTipoSala.Descripcion = updatedTipoSalas.Descripcion;


            await _serviceTipoSalas.UpdateAsync(existingTipoSala);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoSala(int id)
        {
            var tipoSala = await _serviceTipoSalas.GetByIdAsync(id);
            if (tipoSala == null)
            {
                return NotFound();
            }
            await _serviceTipoSalas.DeleteAsync(id);
            return NoContent();
        }

    }
}