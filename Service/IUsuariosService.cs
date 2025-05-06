using Models;
using CoWorking.DTO;
using System.Security.Claims;

namespace CoWorking.Service
{
    public interface IUsuariosService
    {
        Task<List<Usuarios>> GetAllAsync();
        Task<Usuarios?> GetByIdAsync(int id);
        Task AddAsync(Usuarios usuario);
        Task UpdateAsync(Usuarios usuario);
        Task DeleteAsync(int id);
       
        Task<List<UsuarioClienteDTO>> GetClientesByEmailAsync(string email);
         /*
        Task<Usuarios?> GetUsuarioFromJwtAsync(ClaimsPrincipal user);
        Task<bool> ChangeUserRoleAsync(string email);
        Task<bool> QuitarAdminAsync(string email);
        */
    }
}