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

    Task<List<SalasConCaracteristicasDTO>> GetAllWithCaracteristicasAsync();
    Task AddCaracteristicaToSalaAsync(int idSala, int idCaracteristica);
    Task RemoveCaracteristicaFromSalaAsync(int idSala, int idCaracteristica);
    Task<List<CaracteristicaSalaDTO>> GetCaracteristicasBySalaAsync(int idSala);

    Task<string?> GetSalaNameByPuestoTrabajoIdAsync(int idPuestoTrabajo);
    Task<decimal?> GetPrecioPuestoTrabajoAsync(int idPuestoTrabajo);


  }

}