using CoWorking.Repositories;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public class EstadisticasService : IEstadisticasService
    {
        private readonly IEstadisticasRepository _estadisticasRepository;

        public EstadisticasService(IEstadisticasRepository estadisticasRepository)
        {
            _estadisticasRepository = estadisticasRepository;
        }

        public async Task<DashboardEstadisticasDTO> GetDashboardEstadisticasAsync()
        {
            return await _estadisticasRepository.GetDashboardEstadisticasAsync();
        }
    }
}
