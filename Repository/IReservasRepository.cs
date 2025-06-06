using Models;
using CoWorking.DTO;
using Models.DTOs;

namespace CoWorking.Repositories
{
    public interface IReservasRepository
    {
        Task<List<ReservasDTO>> GetAllAsync();
        Task<ReservasDTO> GetByIdAsync(int id);
        Task<Reservas> CreateReservaAsync(Reservas reserva);


        Task UpdateAsync(ReservasUpdateDTO reservas);

        Task DeleteAsync(int id);

        Task<List<GetReservasClienteDTO>> GetReservasUsuarioAsync(int idUsuario);

        Task<GetDetallesReservaDTO> GetResumenReservaAsync(int id);
        Task<Reservas> CreateReservaConLineasAsync(ReservaPostDTO reservaDTO);

        Task<bool> ValidarReservaExisteQR(int idReserva, int idUsuario, DateTime fecha);


    }
}