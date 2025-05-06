using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public interface ICaracteristicasSalaRepository
    {
        Task<List<CaracteristicasSala>> GetAllAsync();
        Task<CaracteristicasSala?> GetByIdAsync(int id);
        Task AddAsync(CaracteristicasSala caracteristicaSala);
        Task UpdateAsync(CaracteristicasSala caracteristicaSala);
        Task DeleteAsync(int id);

    }
}
