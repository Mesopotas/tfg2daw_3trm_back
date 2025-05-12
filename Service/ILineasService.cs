using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface ILineasService
    {
        Task<List<LineasDTO>> GetAllAsync();
        Task<LineasDTO?> GetByIdAsync(int id);
        Task AddAsync(LineasDTO linea);
        Task DeleteAsync(int id);
    }
}