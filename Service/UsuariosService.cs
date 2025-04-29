using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoWorking.Repositories;
using CoWorking.DTO;
using CoWorking.Service;
using System.Security.Claims;

namespace CoWorking.Service
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuariosRepository _usuariosRepository;

        public UsuariosService(IUsuariosRepository usuariosRepository)
        {
            _usuariosRepository = usuariosRepository;
        }

        public async Task<List<Usuarios>> GetAllAsync()
        {
            return await _usuariosRepository.GetAllAsync();
        }

        public async Task<Usuarios?> GetByIdAsync(int id)
        {
            return await _usuariosRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Usuarios usuario)
        {
            await _usuariosRepository.AddAsync(usuario);
        }

        public async Task UpdateAsync(Usuarios usuario)
        {
            await _usuariosRepository.UpdateAsync(usuario);
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _usuariosRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                //return NotFound();
            }
            await _usuariosRepository.DeleteAsync(id);
            //return NoContent();
        }

        public async Task<List<UsuarioClienteDTO>> GetClientesByEmailAsync(string email)
        {
            return await _usuariosRepository.GetClientesByEmailAsync(email);
        }
        // metodo para hacer elget por id pero sin tener que completarlo manual, sino que lo extrae del JWT
        public async Task<Usuarios> GetUsuarioFromJwtAsync(ClaimsPrincipal user)
        {
            // El claim 'NameIdentifier' contiene el ID del usuario
            var idUsuarioClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idUsuarioClaim is null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario) /*  transforma el valor del claim en int para el get*/)
            //si no encuentra el claim o no lo puede convertir a int, devolverá null
            {
                return null; // no se pudo obtener el ID del usuario autenticado
            }
            
            return await _usuariosRepository.GetByIdAsync(idUsuario); // obtener el usuario del id del claim
        }
           public async Task<bool> ChangeUserRoleAsync(string email)
        {
            return await _usuariosRepository.ChangeUserRoleAsync(email);
        }
              public async  Task<bool> QuitarAdminAsync(string email)        {
            return await _usuariosRepository.QuitarAdminAsync(email);
        }
    }
}