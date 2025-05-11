using Models;
using CoWorking.DTO;
using Models.DTOs;

namespace CoWorking.Service
{
    public interface IReservasService
    {
        Task<List<ReservasDTO>> GetAllAsync();
        Task<ReservasDTO> GetByIdAsync(int id);
        Task CreateReservaAsync(Reservas reserva);
        Task UpdateAsync(ReservasUpdateDTO reserva);
        //Task DeleteAsync(int id);
        Task<List<GetReservasClienteDTO>> GetReservasUsuario(int idUsuario);


    }
}