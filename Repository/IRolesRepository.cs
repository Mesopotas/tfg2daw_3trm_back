using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface IRolesRepository
    {
        Task<List<Roles>> GetAllAsync();
        Task<Roles?> GetByIdAsync(int id);
        Task AddAsync(Roles roles);
        Task UpdateAsync(Roles roles);
        Task DeleteAsync(int id);

    }
}