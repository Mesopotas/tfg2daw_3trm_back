using Models;
using CoWorking.DTO;
using System.Security.Claims;

namespace CoWorking.Service
{
    public interface ICaracteristicasSalaService
    {
        Task<List<CaracteristicasSala>> GetAllAsync();
        Task<CaracteristicasSala?> GetByIdAsync(int id);
        Task AddAsync(CaracteristicasSala caracteristicaSala);
        Task UpdateAsync(CaracteristicasSala caracteristicaSala);
        Task DeleteAsync(int id);
        /*
        Task<List<UsuarioClienteDTO>> GetClientesByEmailAsync(string email);
        Task<Usuarios?> GetUsuarioFromJwtAsync(ClaimsPrincipal user);
        Task<bool> ChangeUserRoleAsync(string email);
        Task<bool> QuitarAdminAsync(string email);
        */
    }
}