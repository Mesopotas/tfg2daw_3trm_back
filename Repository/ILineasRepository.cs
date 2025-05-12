using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface ILineasRepository
    {
        Task<List<LineasDTO>> GetAllAsync();
        Task<LineasDTO?> GetByIdAsync(int id);
        Task AddAsync(LineasDTO linea);
        Task DeleteAsync(int id);

    }
}