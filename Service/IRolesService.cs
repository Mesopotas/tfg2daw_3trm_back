using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface IRolesService
    {
        Task<List<Roles>> GetAllAsync();
        Task<Roles?> GetByIdAsync(int id);
        Task AddAsync(Roles rol);
        Task UpdateAsync(Roles rol);
        Task DeleteAsync(int id);
    }
}