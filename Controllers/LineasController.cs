using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineasController : ControllerBase
    {
        private static List<Lineas> detalleLineas = new List<Lineas>();

        private readonly ILineasService _serviceLineas;

        public LineasController(ILineasService service)
        {
            _serviceLineas = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Lineas>>> GetLineas()
        {
            var lineas = await _serviceLineas.GetAllAsync();
            return Ok(lineas);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Lineas>> GetLinea(int id)
        {
            var detalleLineas = await _serviceLineas.GetByIdAsync(id);
            if (detalleLineas == null)
            {
                return NotFound();
            }
            return Ok(detalleLineas);
        }


        [HttpPost]
        public async Task<ActionResult<LineasDTO>> CreateLinea(LineasDTO lineas)
        {
            await _serviceLineas.AddAsync(lineas);
            return CreatedAtAction(nameof(CreateLinea), new { id = lineas.IdLinea }, lineas);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLinea(int id)
        {
            var detalleLineas = await _serviceLineas.GetByIdAsync(id);
            if (detalleLineas == null)
            {
                return NotFound();
            }
            await _serviceLineas.DeleteAsync(id);
            return NoContent();
        }

    }
}