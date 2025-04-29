using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface ITramosHorariosService
    {
        Task<List<TramosHorarios>> GetAllAsync();
        Task<TramosHorarios?> GetByIdAsync(int id);
        Task AddAsync(TramosHorarios tramoHorario);
        Task UpdateAsync(TramosHorarios tramoHorario);
        Task DeleteAsync(int id);
    }
}