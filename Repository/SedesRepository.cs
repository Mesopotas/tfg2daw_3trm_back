using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public class SedesRepository : ISedesRepository
    {
        private readonly string _connectionString;

        public SedesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Sedes>> GetAllAsync()
        {
            var sedes = new List<Sedes>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdSede, Pais, Ciudad, Direccion, CodigoPostal, Planta, Observaciones FROM Sedes";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var sede = new Sedes
                            {
                                IdSede = reader.GetInt32(0),
                                Pais = reader.GetString(1),
                                Ciudad = reader.GetString(2),
                                Direccion = reader.GetString(3),
                                CodigoPostal = reader.GetString(4),
                                Planta = reader.GetString(5),
                                Observaciones = reader.GetString(6),
                            };

                            sedes.Add(sede);
                        }
                    }
                }
            }
            return sedes;
        }

        public async Task<Sedes> GetByIdAsync(int id)
        {
            Sedes sede = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdSede, Pais, Ciudad, Direccion, CodigoPostal, Planta, Observaciones FROM Sedes WHERE idSede = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            sede = new Sedes
                            {
                                IdSede = reader.GetInt32(0),
                                Pais = reader.GetString(1),
                                Ciudad = reader.GetString(2),
                                Direccion = reader.GetString(3),
                                CodigoPostal = reader.GetString(4),
                                Planta = reader.GetString(5),
                                Observaciones = reader.GetString(6),
                            };

                        }
                    }
                }
            }
            return sede;
        }

        public async Task AddAsync(Sedes sede)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Sedes (Pais, Ciudad, Direccion, CodigoPostal, Planta, Observaciones) VALUES (@Pais, @Ciudad, @Direccion, @CodigoPostal, @Planta, @Observaciones)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Pais", sede.Pais);
                    command.Parameters.AddWithValue("@Ciudad", sede.Ciudad);
                    command.Parameters.AddWithValue("@Direccion", sede.Direccion);
                    command.Parameters.AddWithValue("@CodigoPostal", sede.CodigoPostal);
                    command.Parameters.AddWithValue("@Planta", sede.Planta);
                    command.Parameters.AddWithValue("@Observaciones", sede.Observaciones);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAsync(Sedes sede)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Sedes SET Pais = @Pais, Ciudad = @Ciudad, Direccion = @Direccion, CodigoPostal = @CodigoPostal, Planta = @Planta, Observaciones = @Observaciones WHERE idSede = @IdSede";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdSede", sede.IdSede);
                    command.Parameters.AddWithValue("@Pais", sede.Pais);
                    command.Parameters.AddWithValue("@Ciudad", sede.Ciudad);
                    command.Parameters.AddWithValue("@Direccion", sede.Direccion);
                    command.Parameters.AddWithValue("@CodigoPostal", sede.CodigoPostal);
                    command.Parameters.AddWithValue("@Planta", sede.Planta);
                    command.Parameters.AddWithValue("@Observaciones", sede.Observaciones);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Sedes WHERE idSede = @IdSede";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdSede", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}