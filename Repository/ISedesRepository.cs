using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface ISedesRepository
    {
        Task<List<Sedes>> GetAllAsync();
        Task<Sedes?> GetByIdAsync(int id);
        Task AddAsync(Sedes sedes);
        Task UpdateAsync(Sedes sedes);
        Task DeleteAsync(int id);

    }
}