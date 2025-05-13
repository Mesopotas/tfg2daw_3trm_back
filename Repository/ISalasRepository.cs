using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{


      public interface ISalasRepository
    {
        Task<List<SalasDTO>> GetAllAsync();
        Task<SalasDTO?> GetByIdAsync(int id);
        Task AddAsync(SalasDTO sala);
        Task UpdateAsync(SalasDTO sala);
        Task DeleteAsync(int id);
        Task<List<SalasDTO>> GetByIdSedeAsync(int idSede);

    }

  /*
    public interface ISalasRepository
    {
        Task<List<SalasDetallesDTO>> GetAllAsync();
        Task<SalasDetallesDTO> GetByIdAsync(int id);
        Task<List<SalasDTO>> GetByIdSedeAsync(int id);
        Task AddAsync(Salas salas);
      //  Task UpdateAsync(Salas salas);
        Task DeleteAsync(int id);

    }

  */  
}