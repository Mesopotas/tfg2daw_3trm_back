
using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface ITramosHorariosRepository
    {
        Task<List<TramosHorarios>> GetAllAsync();
        Task<TramosHorarios?> GetByIdAsync(int id);
        Task AddAsync(TramosHorarios tramosHorarios);
        Task UpdateAsync(TramosHorarios tramosHorarios);
        Task DeleteAsync(int id);

    }
}