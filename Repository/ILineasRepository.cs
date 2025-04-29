using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface ILineasRepository
    {
        Task<List<Lineas>> GetAllAsync();
        Task<Lineas?> GetByIdAsync(int id);
        Task AddAsync(Lineas lineas);
        Task DeleteAsync(int id);

    }
}