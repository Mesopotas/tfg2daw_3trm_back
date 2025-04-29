using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;

namespace CoWorking.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly string _connectionString;

        public RolesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Roles>> GetAllAsync()
        {
            var roles = new List<Roles>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdRol, Nombre, Descripcion FROM Roles";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var rol = new Roles
                            {
                                IdRol = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2)
                            };

                            roles.Add(rol);
                        }
                    }
                }
            }
            return roles;
        }

        public async Task<Roles> GetByIdAsync(int id)
        {
            Roles rol = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdRol, Nombre, Descripcion FROM Roles WHERE idRol = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            rol = new Roles
                            {
                                IdRol = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2)
                            };

                        }
                    }
                }
            }
            return rol;
        }

        public async Task AddAsync(Roles rol)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO Roles (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", rol.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", rol.Descripcion);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAsync(Roles rol)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Roles SET Nombre = @Nombre, Descripcion = @Descripcion WHERE idRol = @IdRol";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdRol", rol.IdRol);
                    command.Parameters.AddWithValue("@Nombre", rol.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", rol.Descripcion);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Roles WHERE idRol = @IdRol";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdRol", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}