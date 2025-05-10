using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposSalasController : ControllerBase
    {
        private static List<TiposSalas> TiposSalas = new List<TiposSalas>();

        private readonly ITiposSalasService _serviceTiposSalas;

        public TiposSalasController(ITiposSalasService service)
        {
            _serviceTiposSalas = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TiposSalas>>> GetTiposSalas()
        {
            var TiposSalas = await _serviceTiposSalas.GetAllAsync();
            return Ok(TiposSalas);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TiposSalas>> GetTipoSala(int id)
        {
            var tipoSala = await _serviceTiposSalas.GetByIdAsync(id);
            if (tipoSala == null)
            {
                return NotFound();
            }
            return Ok(tipoSala);
        }


        [HttpPost]
        public async Task<ActionResult<TiposSalas>> CreateTipoSala(TiposSalas TiposSalas)
        {
            await _serviceTiposSalas.AddAsync(TiposSalas);
            return CreatedAtAction(nameof(CreateTipoSala), new { id = TiposSalas.IdTipoSala }, TiposSalas);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTipoSala(int id, TiposSalas updatedTiposSalas)
        {
            var existingTipoSala = await _serviceTiposSalas.GetByIdAsync(id);
            if (existingTipoSala == null)
            {
                return NotFound();
            }
            existingTipoSala.Descripcion = updatedTiposSalas.Descripcion;


            await _serviceTiposSalas.UpdateAsync(existingTipoSala);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoSala(int id)
        {
            var tipoSala = await _serviceTiposSalas.GetByIdAsync(id);
            if (tipoSala == null)
            {
                return NotFound();
            }
            await _serviceTiposSalas.DeleteAsync(id);
            return NoContent();
        }

    }
}