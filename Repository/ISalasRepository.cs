using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface ISalasRepository
    {
        Task<List<SalasDetallesDTO>> GetAllAsync();
        Task<SalasDetallesDTO> GetByIdAsync(int id);
        Task<List<SalasDTO>> GetByIdSedeAsync(int id);
        Task AddAsync(Salas salas);
      //  Task UpdateAsync(Salas salas);
        Task DeleteAsync(int id);

    }
}