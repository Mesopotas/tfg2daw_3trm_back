using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface ITiposPuestosTrabajoService
    {
        Task<List<TiposPuestosTrabajo>> GetAllAsync();
        Task<TiposPuestosTrabajo?> GetByIdAsync(int id);
        Task AddAsync(TiposPuestosTrabajo tipoPuestoTrabajo);
        Task UpdateAsync(TiposPuestosTrabajo tipoPuestoTrabajo);
        Task DeleteAsync(int id);
    }
}