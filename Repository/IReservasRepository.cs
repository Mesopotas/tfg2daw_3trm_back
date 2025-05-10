using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface IReservasRepository
    {
        Task<List<ReservasDTO>> GetAllAsync();
        Task<ReservasDTO> GetByIdAsync(int id);
        Task<Reservas> CreateReservaAsync(Reservas reserva);


       // Task UpdateAsync(Reservas reservas);
    //    Task DeleteAsync(int id);

 //    Task<ReservasClienteInfoDTO> GetDetallesPedidoAsync(int idReserva);

    }
}