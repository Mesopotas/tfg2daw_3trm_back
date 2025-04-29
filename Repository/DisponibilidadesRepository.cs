using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using System.Data;

namespace CoWorking.Repositories
{
    public class DisponibilidadesRepository : IDisponibilidadesRepository
    {
        private readonly string _connectionString;

        public DisponibilidadesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        /*    public int IdDisponibilidad { get; set; }
                public int Fecha { get; set; }
                public bool Estado { get; set; }*/
        public async Task<List<DisponibilidadDTO>> GetAllAsync()
        {
            var disponibilidades = new List<DisponibilidadDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdDisponibilidad, Fecha, Estado FROM Disponibilidades WHERE fecha >= DAY(GETDATE());";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var disponibilidad = new DisponibilidadDTO
                            {
                                IdDisponibilidad = reader.GetInt32(0),
                                Fecha = reader.GetInt32(1),
                                Estado = reader.GetBoolean(2)
                            };

                            disponibilidades.Add(disponibilidad);
                        }
                    }
                }
            }
            return disponibilidades;
        }

        public async Task<DisponibilidadDTO> GetByIdAsync(int id)
        {
            DisponibilidadDTO disponibilidad = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT IdDisponibilidad, Fecha, Estado FROM Disponibilidades WHERE IdDisponibilidad = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            disponibilidad = new DisponibilidadDTO
                            {
                                IdDisponibilidad = reader.GetInt32(0),
                                Fecha = reader.GetInt32(1),
                                Estado = reader.GetBoolean(2)
                            };

                        }
                    }
                }
            }
            return disponibilidad;
        }




        public async Task<List<DisponibilidadDTO>> GetByIdPuestoTrabajoAsync(int id)
        {
            List<DisponibilidadDTO> disponibilidades = new List<DisponibilidadDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT IdDisponibilidad, Fecha, Estado, IdTramoHorario FROM Disponibilidades WHERE IdPuestoTrabajo = @Id  AND fecha >= DAY(GETDATE());;";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var disponibilidad = new DisponibilidadDTO
                            {
                                IdDisponibilidad = reader.GetInt32(0),
                                Fecha = reader.GetInt32(1),
                                Estado = reader.GetBoolean(2),
                                IdTramoHorario = reader.GetInt32(3)
                            };

                            disponibilidades.Add(disponibilidad);
                        }
                    }
                }
            }
            return disponibilidades;
        }




        public async Task UpdateDisponibilidadAsync(DisponibilidadDTO disponibilidad)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Disponibilidades SET Estado = @Estado WHERE IdDisponibilidad = @IdDisponibilidad";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdDisponibilidad", disponibilidad.IdDisponibilidad);
                    command.Parameters.AddWithValue("@Estado", false);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

    }
}