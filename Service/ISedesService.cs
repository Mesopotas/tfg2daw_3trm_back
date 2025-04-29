using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface ISedesService
    {
        Task<List<Sedes>> GetAllAsync();
        Task<Sedes?> GetByIdAsync(int id);
        Task AddAsync(Sedes sede);
        Task UpdateAsync(Sedes sede);
        Task DeleteAsync(int id);
    }
}