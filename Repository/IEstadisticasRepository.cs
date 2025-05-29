using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface IEstadisticasRepository
    {
        Task<DashboardEstadisticasDTO> GetDashboardEstadisticasAsync();
    }
}