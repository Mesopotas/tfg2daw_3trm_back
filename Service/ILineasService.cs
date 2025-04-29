using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface ILineasService
    {
        Task<List<Lineas>> GetAllAsync();
        Task<Lineas?> GetByIdAsync(int id);
        Task AddAsync(Lineas linea);
        Task DeleteAsync(int id);
    }
}