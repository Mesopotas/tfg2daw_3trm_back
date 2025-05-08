using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;

namespace CoWorking.Repositories
{
    public class SedesRepository : ISedesRepository
    {
        private readonly CoworkingDBContext _context;

        public SedesRepository(CoworkingDBContext context)
        {
            _context = context;
        }

        public async Task<List<Sedes>> GetAllAsync()
        {
            return await _context.Sedes
            .ToListAsync();
        }

        public async Task<Sedes> GetByIdAsync(int id)
        {
            return await _context.Sedes.FirstOrDefaultAsync(sedes => sedes.IdSede == id); // funcion flecha, usuario recoge todos los usuarios quer cumple que IdUsuario == id
        }

        public async Task AddAsync(Sedes sede)
        {

            await _context.Sedes.AddAsync(sede); // AddAsync es metodo propio de EF, no hace el insert en si, solo lo prepara
            await _context.SaveChangesAsync(); // otro metodo de EF, esto si hace el insert con los datos del add, ambos son imprescindibles para el insert
        }



        public async Task UpdateAsync(Sedes sede)
        {
            _context.Sedes.Update(sede); // igual que el add pero haciendo un update
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var sede = await GetByIdAsync(id); // primero busca el id del usuario
            if (sede != null)
            {// si existe, pasa a ejecutar

                _context.Sedes.Remove(sede); // metodo de EF para eliminar registros (los prepara para eliminacion)
                await _context.SaveChangesAsync();
            }
        }
    }
}