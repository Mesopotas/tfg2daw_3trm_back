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
        Task UpdateAsync(UsuarioUpdateDTO usuario);
        Task DeleteAsync(int id);
        Task<bool> DeleteByEmailAsync(string email);
       
        Task<List<UsuarioClienteDTO>> GetClientesByEmailAsync(string email);
         
        Task<Usuarios?> GetUsuarioFromJwtAsync(ClaimsPrincipal user);
        
        Task<bool> ChangeUserRoleAsync(string email);
        Task<bool> QuitarAdminAsync(string email);
    }
}