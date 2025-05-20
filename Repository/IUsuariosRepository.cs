using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface IUsuariosRepository
    {
        Task<List<Usuarios>> GetAllAsync();
        Task<Usuarios?> GetByIdAsync(int id);
        Task AddAsync(Usuarios usuario);
        Task UpdateAsync(UsuarioUpdateDTO usuario);
        Task DeleteAsync(int id);
     
        Task<List<UsuarioClienteDTO>> GetClientesByEmailAsync(string email);
   
         Task<UserDTOOut> GetUserFromCredentialsAsync(LoginDto login);
   
         Task<UserDTOOut> AddUserFromCredentialsAsync(RegisterDTO register);
         
        Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO);

        Task<bool> ChangeUserRoleAsync(string email);

        Task<bool> QuitarAdminAsync(string email);
        
    }
}
