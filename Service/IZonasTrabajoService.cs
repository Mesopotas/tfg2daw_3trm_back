using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{

      public interface IZonasTrabajoService
    {
        Task<List<ZonasTrabajo>> GetAllAsync();
        Task<ZonasTrabajo?> GetByIdAsync(int id);
        Task AddAsync(ZonasTrabajoDTO zonaTrabajoDTO);
        Task UpdateAsync(ZonasTrabajo zonaTrabajo);
        Task DeleteAsync(int id);
    }
    }