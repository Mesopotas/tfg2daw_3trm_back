using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface IEstadisticasService
    {
        Task<DashboardEstadisticasDTO> GetDashboardEstadisticasAsync();

    }
}
