using Models;
using CoWorking.DTO;
using Dtos;

namespace CoWorking.Service
{
    public interface IPuestosTrabajoService
    {
        Task<List<PuestosTrabajoDTO>> GetAllAsync();
        Task<PuestosTrabajoDTO?> GetByIdAsync(int id);
        Task AddAsync(PuestosTrabajoDTO puestoTrabajo);
        Task UpdateAsync(PuestosTrabajoDTO puestoTrabajo);
        Task DeleteAsync(int id);

        Task<List<PuestosTrabajoDTO>> GetDisponiblesEnSedeAsync(int idSede, DateTime fechaInicio, DateTime fechaFin, TimeSpan horaInicio, TimeSpan horaFin);

    }
}