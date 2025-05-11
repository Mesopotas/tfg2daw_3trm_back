using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public class LineasRepository : ILineasRepository
    {
        private readonly string _connectionString;

        public LineasRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Lineas>> GetAllAsync()
        {
            var lineas = new List<Lineas>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdLinea, IdReserva, Precio FROM Lineas";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var linea = new Lineas
                            {
                                IdLinea = reader.GetInt32(0),
                                IdReserva = reader.GetInt32(1),
                                Precio = reader.GetDecimal(2)
                            };

                            lineas.Add(linea);
                        }
                    }
                }
            }
            return lineas;
        }

        public async Task<Lineas> GetByIdAsync(int id)
        {
            Lineas linea = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdLinea, IdReserva, Precio FROM Lineas WHERE IdLinea = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            linea = new Lineas
                            {
                                IdLinea = reader.GetInt32(0),
                                IdReserva = reader.GetInt32(1),
                                Precio = reader.GetDecimal(2)
                            };
                        }
                    }
                }
            }
            return linea;
        }

public async Task AddAsync(Lineas linea)
{
    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        string queryGetPrecio = "SELECT PrecioTotal FROM Reservas WHERE IdReserva = @IdReserva"; // obtener y asignar el precio automaticamente
        using (var commandGet = new SqlCommand(queryGetPrecio, connection))
        {
            commandGet.Parameters.AddWithValue("@IdReserva", linea.IdReserva);
            object result = await commandGet.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
            {
        linea.Precio = (decimal)result;
            }
        }

        // insert con el precio obtenido una vez le asigne id de reserva
        string queryInsert = "INSERT INTO Lineas (IdReserva, Precio) VALUES (@IdReserva, @Precio)";
        using (var commandInsert = new SqlCommand(queryInsert, connection))
        {
            commandInsert.Parameters.AddWithValue("@IdReserva", linea.IdReserva);
            commandInsert.Parameters.AddWithValue("@Precio", linea.Precio);

            await commandInsert.ExecuteNonQueryAsync();
        }
    }
}



        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Lineas WHERE IdLinea = @IdLinea";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdLinea", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
