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
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE TramosHorarios SET HoraInicio = @HoraInicio, HoraFin = @HoraFin,  DIASEMANAL = @DiaSemanal WHERE idTramoHorario = @IdTramosHorario";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@HoraInicio", tramosHorario.HoraInicio);
                    command.Parameters.AddWithValue("@HoraFin", tramosHorario.HoraFin);
                    //   command.Parameters.AddWithValue("@DiaSemanal", tramosHorario.DiaSemanal);
                    await command.ExecuteNonQueryAsync();
                }
            }
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