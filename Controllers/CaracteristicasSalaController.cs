using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;
using Microsoft.AspNetCore.Authorization;
using CaracteristicasSalaModel = Models.CaracteristicasSala;



namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaracteristicasSalaController : ControllerBase
    {
        private static List<CaracteristicasSalaModel> caracteristicasSala = new List<CaracteristicasSala>();

        private readonly ICaracteristicasSalaService _serviceCaracteristicasSala;

        public CaracteristicasSalaController(ICaracteristicasSalaService service)
        {
            _serviceCaracteristicasSala = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<CaracteristicasSalaModel>>> GetCaracteristicasSala()
        {
            var caracteristicasSala = await _serviceCaracteristicasSala.GetAllAsync();
            return Ok(caracteristicasSala);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CaracteristicasSalaModel>> GetCaracteristicaSala(int id)
        {
            var caracteristicaSala = await _serviceCaracteristicasSala.GetByIdAsync(id);
            if (caracteristicaSala == null)
            {
                return NotFound();
            }
            return Ok(caracteristicaSala);
        }


        [HttpPost]
        public async Task<ActionResult<CaracteristicasSalaModel>> CreateCaracteristicaSala(CaracteristicasSalaModel caracteristicasSala)
        {
            await _serviceCaracteristicasSala.AddAsync(caracteristicasSala);
            return CreatedAtAction(nameof(CreateCaracteristicaSala), new { id = caracteristicasSala.IdCaracteristica }, caracteristicasSala);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCaracteristicaSala(int id, CaracteristicasSalaModel updatedCaracteristicasSala)
        {
            var existingCaracteristicaSala = await _serviceCaracteristicasSala.GetByIdAsync(id);
            if (existingCaracteristicaSala == null)
            {
                return NotFound();
            }
            existingCaracteristicaSala.Nombre = updatedCaracteristicasSala.Nombre;
            existingCaracteristicaSala.Descripcion = updatedCaracteristicasSala.Descripcion;
            existingCaracteristicaSala.PrecioAniadido = updatedCaracteristicasSala.PrecioAniadido;

            await _serviceCaracteristicasSala.UpdateAsync(existingCaracteristicaSala);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCaracteristicaSala(int id)
        {
            var caracteristicaSala = await _serviceCaracteristicasSala.GetByIdAsync(id);
            if (caracteristicaSala == null)
            {
                return NotFound();
            }
            await _serviceCaracteristicasSala.DeleteAsync(id);
            return NoContent();
        }
    }
}