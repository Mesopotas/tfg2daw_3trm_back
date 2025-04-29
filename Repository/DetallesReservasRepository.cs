using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public class DetallesReservasRepository : IDetallesReservasRepository
    {
        private readonly string _connectionString;

        public DetallesReservasRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<DetallesReservas>> GetAllAsync()
        {
            var detallesReservas = new List<DetallesReservas>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdDetalleReserva, Descripcion, IdPuestoTrabajo FROM DetallesReservas";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var detallesReserva = new DetallesReservas
                            {
                                IdDetalleReserva = reader.GetInt32(0),
                                Descripcion = reader.GetString(1),
                                IdPuestoTrabajo = reader.GetInt32(2)

                            };

                            detallesReservas.Add(detallesReserva);
                        }
                    }
                }
            }
            return detallesReservas;
        }

        public async Task<DetallesReservas> GetByIdAsync(int id)
        {
            DetallesReservas detallesReserva = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdDetalleReserva, Descripcion, IdPuestoTrabajo FROM DetallesReservas WHERE idDetalleReserva = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            detallesReserva = new DetallesReservas
                            {
                                IdDetalleReserva = reader.GetInt32(0),
                                Descripcion = reader.GetString(1),
                                IdPuestoTrabajo = reader.GetInt32(2)

                            };

                        }
                    }
                }
            }
            return detallesReserva;
        }

public async Task AddAsync(DetallesReservas detallesReserva)
{
    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        string query = "INSERT INTO DetallesReservas (Descripcion, IdPuestoTrabajo) VALUES (@Descripcion, @IdPuestoTrabajo); SELECT SCOPE_IDENTITY();";

        using (var command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Descripcion", detallesReserva.Descripcion);
            command.Parameters.AddWithValue("@IdPuestoTrabajo", detallesReserva.IdPuestoTrabajo);
            
            // Obtain SCOPE_IDENTITY() value (last inserted ID)
            var newDetalleReservaId = await command.ExecuteScalarAsync();
            if (newDetalleReservaId != null && newDetalleReservaId != DBNull.Value)
            {
                detallesReserva.IdDetalleReserva = Convert.ToInt32(newDetalleReservaId);
            }
        }
        // Connection will be automatically closed here due to the using statement
    }
}

        public async Task UpdateAsync(DetallesReservas detallesReserva)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // La columna FechaRegistro no está incluida ya que no debe ser modificada
                string query = "UPDATE DetallesReservas SET Descripcion = @Descripcion WHERE idDetalleReserva = @IdDetallesReserva";
                // si el idRol asignado no existe dará error (Microsoft.Data.SqlClient.SqlException (0x80131904): The INSERT statement conflicted with the FOREIGN KEY constraint "FK__DetallesReservas__IdRol__276EDEB3". The conflict occurred in database "CoworkingDB", table "dbo.Roles", column 'IdRol'.)

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", detallesReserva.Descripcion);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<int?> GetLastIdAsync()
        {
            int? lastId = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Consulta para obtener el último IdDetalleReserva insertado
                string query = "SELECT MAX(IdDetalleReserva) FROM DetallesReservas";

                using (var command = new SqlCommand(query, connection))
                {
                    var result = await command.ExecuteScalarAsync();

                    if (result != DBNull.Value)
                    {
                        lastId = Convert.ToInt32(result);
                    }
                }
            }

            return lastId;
        }
        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM DetallesReservas WHERE idDetalleReserva = @IdDetalleReserva";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdDetalleReserva", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}