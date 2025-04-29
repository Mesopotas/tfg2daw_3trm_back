using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;

namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private static List<Roles> rol = new List<Roles>();

        private readonly IRolesService _serviceRoles;

        public RolesController(IRolesService service)
        {
            _serviceRoles = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Roles>>> GetRoles()
        {
            var roles = await _serviceRoles.GetAllAsync();
            return Ok(roles);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Roles>> GetRole(int id)
        {
            var rol = await _serviceRoles.GetByIdAsync(id);
            if (rol == null)
            {
                return NotFound();
            }
            return Ok(rol);
        }


        [HttpPost]
        public async Task<ActionResult<Roles>> CreateRole(Roles roles)
        {
            await _serviceRoles.AddAsync(roles);
            return CreatedAtAction(nameof(CreateRole), new { id = roles.IdRol }, roles);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, Roles updatedRoles)
        {
            var existingRole = await _serviceRoles.GetByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound();
            }
            existingRole.Nombre = updatedRoles.Nombre;
            existingRole.Descripcion = updatedRoles.Descripcion;


            await _serviceRoles.UpdateAsync(existingRole);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var rol = await _serviceRoles.GetByIdAsync(id);
            if (rol == null)
            {
                return NotFound();
            }
            await _serviceRoles.DeleteAsync(id);
            return NoContent();
        }

    }
}