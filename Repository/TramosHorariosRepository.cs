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
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM TramosHorarios WHERE idTramoHorario = @IdTramoHorario";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTramoHorario", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}