using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;
using Microsoft.AspNetCore.Authorization;
using TiposPuestosTrabajoModel = Models.TiposPuestosTrabajo;



namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposPuestosTrabajoController : ControllerBase
    {
        private static List<TiposPuestosTrabajoModel> tiposPuestosTrabajo = new List<TiposPuestosTrabajo>();

        private readonly ITiposPuestosTrabajoService _serviceTiposPuestosTrabajo;

        public TiposPuestosTrabajoController(ITiposPuestosTrabajoService service)
        {
            _serviceTiposPuestosTrabajo = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<TiposPuestosTrabajoModel>>> GetTiposPuestosTrabajo()
        {
            var tiposPuestosTrabajo = await _serviceTiposPuestosTrabajo.GetAllAsync();
            return Ok(tiposPuestosTrabajo);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TiposPuestosTrabajoModel>> GetTipoPuestoTrabajo(int id)
        {
            var tipoPuestoTrabajo = await _serviceTiposPuestosTrabajo.GetByIdAsync(id);
            if (tipoPuestoTrabajo == null)
            {
                return NotFound();
            }
            return Ok(tipoPuestoTrabajo);
        }


        [HttpPost]
        public async Task<ActionResult<TiposPuestosTrabajoModel>> CreateTipoPuestoTrabajo(TiposPuestosTrabajoModel tiposPuestosTrabajo)
        {
            await _serviceTiposPuestosTrabajo.AddAsync(tiposPuestosTrabajo);
            return CreatedAtAction(nameof(CreateTipoPuestoTrabajo), new { id = tiposPuestosTrabajo.IdTipoPuestoTrabajo }, tiposPuestosTrabajo);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTipoPuestoTrabajo(int id, TiposPuestosTrabajoModel updatedTipoPuestoTrabajo)
        {
            var existingTipoPuestoTrabajo = await _serviceTiposPuestosTrabajo.GetByIdAsync(id);
            if (existingTipoPuestoTrabajo == null)
            {
                return NotFound();
            }
            existingTipoPuestoTrabajo.Nombre = existingTipoPuestoTrabajo.Nombre;
            existingTipoPuestoTrabajo.Imagen_URL = existingTipoPuestoTrabajo.Imagen_URL;
            existingTipoPuestoTrabajo.Descripcion = existingTipoPuestoTrabajo.Descripcion;
            existingTipoPuestoTrabajo.Precio = existingTipoPuestoTrabajo.Precio;

            await _serviceTiposPuestosTrabajo.UpdateAsync(existingTipoPuestoTrabajo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoPuestoTrabajo(int id)
        {
            var tipoPuestoTrabajo = await _serviceTiposPuestosTrabajo.GetByIdAsync(id);
            if (tipoPuestoTrabajo == null)
            {
                return NotFound();
            }
            await _serviceTiposPuestosTrabajo.DeleteAsync(id);
            return NoContent();
        }
    }
}