using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface IDisponibilidadesService
    {
        Task<List<DisponibilidadDTO>> GetAllAsync();
        Task<DisponibilidadDTO> GetByIdAsync(int id);
        Task<List<DisponibilidadDTO>> GetByIdPuestoTrabajoAsync(int id);
        Task UpdateDisponibilidadAsync(DisponibilidadDTO disponibilidad);
        Task AddDisponibilidadesAsync(int anio);

        Task<List<FechasDisponiblesDTO>> GetDiasBySalaAsync(int salaId);
        Task AddDisponibilidadesParaDiaAsync(DateTime fechaObjetivo);

    }
}