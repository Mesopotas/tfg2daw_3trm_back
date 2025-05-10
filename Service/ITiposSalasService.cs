using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface ITiposSalasService
    {
        Task<List<TiposSalas>> GetAllAsync();
        Task<TiposSalas?> GetByIdAsync(int id);
        Task AddAsync(TiposSalas tipoSala);
        Task UpdateAsync(TiposSalas tipoSala);
        Task DeleteAsync(int id);

    }
}