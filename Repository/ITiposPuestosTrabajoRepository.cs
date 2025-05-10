
using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface ITiposPuestosTrabajoRepository
    {
        Task<List<TiposPuestosTrabajo>> GetAllAsync();
        Task<TiposPuestosTrabajo?> GetByIdAsync(int id);
        Task AddAsync(TiposPuestosTrabajo tiposPuestosTrabajo);
        Task UpdateAsync(TiposPuestosTrabajo tiposPuestosTrabajo);
        Task DeleteAsync(int id);

    }
}