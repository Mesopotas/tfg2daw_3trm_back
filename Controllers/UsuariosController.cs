using Microsoft.AspNetCore.Mvc;
using CoWorking.Repositories;
using CoWorking.Service;
using CoWorking.DTO;
using Models;
using Microsoft.AspNetCore.Authorization;


namespace CoWorking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private static List<Usuarios> usuarios = new List<Usuarios>();

        private readonly IUsuariosService _serviceUsuarios;

        public UsuariosController(IUsuariosService service)
        {
            _serviceUsuarios = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuarios>>> GetUsuarios()
        {
            var usuarios = await _serviceUsuarios.GetAllAsync();
            return Ok(usuarios);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(int id)
        {
            var usuario = await _serviceUsuarios.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }


        [HttpPost]
        public async Task<ActionResult<Usuarios>> CreateUsuario(Usuarios usuarios)
        {
            await _serviceUsuarios.AddAsync(usuarios);
            return CreatedAtAction(nameof(CreateUsuario), new { id = usuarios.IdUsuario }, usuarios);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, UsuarioUpdateDTO updatedUsuario)
        {
            try
            {
                updatedUsuario.IdUsuario = id;

                await _serviceUsuarios.UpdateAsync(updatedUsuario);
                return NoContent(); // 204 OK sin contenido
            }
            catch (InvalidOperationException ex)
            {
                // Email duplicado
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _serviceUsuarios.GetByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            await _serviceUsuarios.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("clientes/{email}")]
        public async Task<ActionResult<List<UsuarioClienteDTO>>> GetClientesById(string email)
        {
            var clientes = await _serviceUsuarios.GetClientesByEmailAsync(email);
            if (clientes.Count == 0)
            {
                return NotFound("No se encontró ninguna cuenta asociada a ese email.");
            }
            return Ok(clientes);
        }

        [HttpGet("byIdConJWT")]
        public async Task<IActionResult> GetUsuarioFromJwt()
        {
            var user = User; // ' User es un ClaimsPrincipal del JWT

            // llamar el nuevo metodo que usa el getbyid sacado de los claims del JWT
            var usuario = await _serviceUsuarios.GetUsuarioFromJwtAsync(user);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            return Ok(usuario);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("cambiar-rol")]
        public async Task<IActionResult> ChangeUserRole(string email)
        {
            var result = await _serviceUsuarios.ChangeUserRoleAsync(email);

            if (!result)
            {
                return NotFound("Usuario no encontrado.");
            }

            return Ok("Rol del usuario cambiado correctamente.");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("quitar-admin")]

        public async Task<IActionResult> QuitarAdminAsync(string email)
        {
            var result = await _serviceUsuarios.QuitarAdminAsync(email);
            if (!result)
            {
                return NotFound("Usuario no encontrado.");
            }

            return Ok("Rol del usuario cambiado correctamente.");
        }

    }
}