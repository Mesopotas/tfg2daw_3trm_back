using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;


namespace CoWorking.Repositories
{
    public class TramosHorariosRepository : ITramosHorariosRepository
    {
        private readonly CoworkingDBContext _context;

        public TramosHorariosRepository(CoworkingDBContext context)
        {
            _context = context;
        }
        public async Task<List<TramosHorarios>> GetAllAsync()
        {
            return await _context.TramosHorarios.ToListAsync();
        }

        public async Task<TramosHorarios> GetByIdAsync(int id)
        {
         return await _context.TramosHorarios.FirstOrDefaultAsync(tramohorario => tramohorario.IdTramoHorario == id); 
        }

        public async Task AddAsync(TramosHorarios tramosHorario)
        {
              await _context.TramosHorarios.AddAsync(tramosHorario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TramosHorarios tramosHorario)
        {
            _context.TramosHorarios.Update(tramosHorario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
               var tramoHorario = await GetByIdAsync(id); // primero busca el id del tramo horario
            if (tramoHorario != null)
            {// si existe, pasa a ejecutar

                _context.TramosHorarios.Remove(tramoHorario); // metodo de EF para eliminar registros (los prepara para eliminacion)
                await _context.SaveChangesAsync();
            }
        }
    }
}