using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface IReservasService
    {
        Task<List<ReservasDTO>> GetAllAsync();
        Task<ReservasDTO> GetByIdAsync(int id);
   /*     Task CreateReservaAsync(Reservas reserva);
      //  Task UpdateAsync(Reservas reserva);
        Task DeleteAsync(int id);
    Task<ReservasClienteInfoDTO> GetDetallesPedido(int idReserva);
*/

    }
}