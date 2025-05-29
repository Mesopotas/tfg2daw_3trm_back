using Microsoft.AspNetCore.Mvc;
using CoWorking.Service;

using CoWorking.DTO;

namespace CoWorking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadisticasController : ControllerBase
    {
        private readonly IEstadisticasService _estadisticasService;

        public EstadisticasController(IEstadisticasService estadisticasService)
        {
            _estadisticasService = estadisticasService;
        }

        [HttpGet("general")]
        public async Task<ActionResult<DashboardEstadisticasDTO>> GetDashboardEstadisticas()
        {
            try
            {
                var estadisticas = await _estadisticasService.GetDashboardEstadisticasAsync();
                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener estadísticas: {ex.Message}");
            }
        }

      
    }
}