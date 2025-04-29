using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface ITipoSalasService
    {
        Task<List<TipoSalas>> GetAllAsync();
        Task<TipoSalas?> GetByIdAsync(int id);
        Task AddAsync(TipoSalas tipoSala);
        Task UpdateAsync(TipoSalas tipoSala);
        Task DeleteAsync(int id);

    }
}