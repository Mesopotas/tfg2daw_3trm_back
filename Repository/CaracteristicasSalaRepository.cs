using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;


namespace CoWorking.Repositories
{
    public class CaracteristicasSalaRepository : ICaracteristicasSalaRepository
    {
        private readonly CoworkingDBContext _context;


        public CaracteristicasSalaRepository(CoworkingDBContext context) // referencia al data.CoworkingDBContext.cs en lugar de cadena de conexión, el EF hará las sentencias sin ponerlas explicitamente
        {
            _context = context;
        }


        public async Task<List<CaracteristicasSala>> GetAllAsync()
        {
            var caracteristicasSala = await _context.CaracteristicasSala

                .Select(u => new CaracteristicasSala
                {
                    IdCaracteristica = u.IdCaracteristica,
                    Nombre = u.Nombre,
                    Descripcion = u.Descripcion,
                    PrecioAniadido = u.PrecioAniadido, // manejo en decimal ahora
                })
                .ToListAsync();

            return caracteristicasSala;
        }


        public async Task<CaracteristicasSala?> GetByIdAsync(int id)
        {
            var caracteristicasSala = await _context.CaracteristicasSala
                .Where(u => u.IdCaracteristica == id)
                .Select(u => new CaracteristicasSala
                {
                    IdCaracteristica = u.IdCaracteristica,
                    Nombre = u.Nombre,
                    Descripcion = u.Descripcion,
                    PrecioAniadido = u.PrecioAniadido, // manejo en decimal ahora
                })
                .FirstOrDefaultAsync();

            return caracteristicasSala;
        }


        public async Task AddAsync(CaracteristicasSala caracteristicaSala)
        {

            await _context.CaracteristicasSala.AddAsync(caracteristicaSala); // AddAsync es metodo propio de EF, no hace el insert en si, solo lo prepara
            await _context.SaveChangesAsync(); // otro metodo de EF, esto si hace el insert con los datos del add, ambos son imprescindibles para el insert
        }


        public async Task UpdateAsync(CaracteristicasSala caracteristicaSala)
        {
            _context.CaracteristicasSala.Update(caracteristicaSala); // igual que el add pero haciendo un update
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var caracteristicaSala = await GetByIdAsync(id); // primero busca el id del usuario
            if (caracteristicaSala != null)
            {// si existe, pasa a ejecutar

                _context.CaracteristicasSala.Remove(caracteristicaSala); // metodo de EF para eliminar registros (los prepara para eliminacion)
                await _context.SaveChangesAsync();
            }
        }
    }
}