using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public class TramosHorariosRepository : ITramosHorariosRepository
    {
        private readonly string _connectionString;

        public TramosHorariosRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<TramosHorarios>> GetAllAsync()
        {
            var tramosHorarios = new List<TramosHorarios>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdTramoHorario, HoraInicio, HoraFin, DIASEMANAL FROM TramosHorarios";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var tramosHorario = new TramosHorarios
                            {
                                IdTramoHorario = reader.GetInt32(0),
                                HoraInicio = reader.GetString(1),
                                HoraFin = reader.GetString(2),
                                DiaSemanal = reader.GetInt32(3),

                            };

                            tramosHorarios.Add(tramosHorario);
                        }
                    }
                }
            }
            return tramosHorarios;
        }

        public async Task<TramosHorarios> GetByIdAsync(int id)
        {
            TramosHorarios tramosHorario = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdTramoHorario, HoraInicio, HoraFin, DIASEMANAL FROM TramosHorarios WHERE idTramoHorario = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            tramosHorario = new TramosHorarios
                            {
                                IdTramoHorario = reader.GetInt32(0),
                                HoraInicio = reader.GetString(1),
                                HoraFin = reader.GetString(2),
                                DiaSemanal = reader.GetInt32(3),

                            };

                        }
                    }
                }
            }
            return tramosHorario;
        }

        public async Task AddAsync(TramosHorarios tramosHorario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO TramosHorarios (HoraInicio, HoraFin, DIASEMANAL) VALUES (@HoraInicio, @HoraFin, @DiaSemanal)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@HoraInicio", tramosHorario.HoraInicio);
                    command.Parameters.AddWithValue("@HoraFin", tramosHorario.HoraFin);
                    command.Parameters.AddWithValue("@DiaSemanal", tramosHorario.DiaSemanal);
                    await command.ExecuteNonQueryAsync();
                }
            }
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
                    command.Parameters.AddWithValue("@DiaSemanal", tramosHorario.DiaSemanal);
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