using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface IZonasTrabajoRepository
    {
        Task<List<ZonasTrabajo>> GetAllAsync();
        Task<ZonasTrabajo?> GetByIdAsync(int id);
        Task AddAsync(ZonasTrabajoDTO zonaTrabajoDTO);
        Task UpdateAsync(ZonasTrabajo zonaTrabajo);
        Task DeleteAsync(int id);

    }
}