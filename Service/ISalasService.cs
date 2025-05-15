using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{

      public interface ISalasService
    {
        Task<List<SalasDTO>> GetAllAsync();
        Task<SalasDTO?> GetByIdAsync(int id);
        Task AddAsync(SalasDTO sala);
        Task UpdateAsync(SalasDTO sala);
        Task DeleteAsync(int id);

        Task<List<SalasDTO>> GetByIdSedeAsync(int idSede);

        Task<List<SalasFiltradoDTO>> GetSalasBySede(int idSede, DateTime fechaInicio, DateTime fechaFin, TimeSpan horaInicio, TimeSpan horaFin);

    }


  /*
    public interface ISalasService
    {
        Task<List<SalasDetallesDTO>> GetAllAsync();
        Task<SalasDetallesDTO> GetByIdAsync(int id);
      Task<List<SalasDTO>> GetByIdSedeAsync(int id);
        Task AddAsync(Salas sala);
      //  Task UpdateAsync(Salas sala);
        Task DeleteAsync(int id);
    }

  */
}