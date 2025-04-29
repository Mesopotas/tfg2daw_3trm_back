using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface IDetallesReservasService
    {
        Task<List<DetallesReservas>> GetAllAsync();
        Task<DetallesReservas?> GetByIdAsync(int id);
        Task<int?> GetLastIdAsync();
        Task AddAsync(DetallesReservas detallesReservas);
        Task UpdateAsync(DetallesReservas detallesReservas);
        Task DeleteAsync(int id);
    }
}